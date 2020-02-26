using System.Collections.Generic;
using System.Linq;

namespace Travelling_Salesman
{
    class GeneticAlgorithmEngine
    {
        // parameters that control different parts of the algorithm ----------------------
        private const int NoChangeGenerationCountForTermination = 100;

        // engine vars and current generation ----------------
        private int populationSize;
        private double crossoverRate;
        private double mutationRate;
        private double elitismRate;
        private int tourneySize;

        private List<CandidateSolution> currentGeneration;
        private int totalFitnessThisGeneration = 0;

        // best-so-far data ----------
        private int shortestPathAllTime = int.MaxValue;
        private CandidateSolution bestSolution = null;
        private int bestSolutionGenerationNumber = 0;

        public GeneticAlgorithmEngine(int initialPopulationSize, int crossoverPercentage, 
            double mutationPercentage, int elitismPercentage, int tournamentSize)
        {
            populationSize = initialPopulationSize;
            crossoverRate = crossoverPercentage / 100D;
            mutationRate = mutationPercentage / 100D;
            elitismRate = elitismPercentage / 100D;
            tourneySize = tournamentSize;
        }

        public PathChosen FindOptimalPath(CityCollection cities)
        {
            // create generation 0 with all randomly created possible solutions
            currentGeneration = new List<CandidateSolution>(populationSize);
            for (int i = 0; i < populationSize; i++)
            {
                currentGeneration.Add(new CandidateSolution(cities));
            }

            int generationNumber = 1;
            while (true)
            {
                totalFitnessThisGeneration = 0;
                int bestFitnessScoreThisGeneration = System.Int32.MinValue;
                CandidateSolution bestSolutionThisGeneration = null;
                
                foreach (var candidate in currentGeneration)
                {
                    candidate.CalculatePathLength();
                }

                // find the longest path, which is the worst solution
                int longestDistance = currentGeneration.Max(c => c.PathLength);

                // then subtract each candidate's distance from that maximum distance
                foreach (var candidate in currentGeneration)
                { 
                    // set fitness so the shortest path is the highest value, and the worst is 0
                    candidate.Fitness = longestDistance - candidate.PathLength;

                    // sum up the fitness scores for our roulette wheel selection
                    totalFitnessThisGeneration += candidate.Fitness;
                    if (candidate.Fitness > bestFitnessScoreThisGeneration)
                    {
                        bestFitnessScoreThisGeneration = candidate.Fitness;
                        bestSolutionThisGeneration = candidate;
                    }
                }

                // Ranked fitnesses for smoothing out selection when fitness scores vary widely

                //currentGeneration = currentGeneration.OrderBy(g => g.Fitness).ToList();
                //int ranking = 1;
                //foreach (var candidate in currentGeneration)
                //{
                //    candidate.Fitness = ranking;
                //    ranking++;
                //}

                int shortestPathThisGeneration = bestSolutionThisGeneration.PathLength;

                // compare this generation's best to the best that have come before it
                if (shortestPathThisGeneration < shortestPathAllTime)
                {
                    // save the best score
                    shortestPathAllTime = shortestPathThisGeneration;

                    // and save the possible solution
                    bestSolution = bestSolutionThisGeneration.DeepClone();
                    bestSolutionGenerationNumber = generationNumber;
                }
                else
                    if ((generationNumber - bestSolutionGenerationNumber) > 
                        NoChangeGenerationCountForTermination)
                    break;

                // create the next generation
                List<CandidateSolution> nextGeneration = new List<CandidateSolution>();

                // Elitism
                int numElitesToAdd = (int) (elitismRate * populationSize);
                var theBest = currentGeneration.OrderBy(c => c.PathLength).Take(numElitesToAdd);
                foreach (var peakPerformer in theBest)
                {
                    nextGeneration.Add(peakPerformer);
                }

                while (nextGeneration.Count < populationSize)
                {
                    CandidateSolution parent1, parent2;

                    // pick two parents based on good fitness
                    parent1 = SelectCandidate();
                    parent2 = SelectCandidate();
                    //parent1 = SelectCandidateViaTournament();
                    //parent2 = SelectCandidateViaTournament();

                    // cross them over to generate two new children
                    CandidateSolution child1, child2;
                    CrossOverParents(parent1, parent2, out child1, out child2);

                    // apply mutation to the children if needed
                    //child1.DoSwapMutation(mutationRate);
                    //child2.DoSwapMutation(mutationRate);
                    child1.DoDisplacementMutation(mutationRate);
                    child2.DoDisplacementMutation(mutationRate);

                    // and then add them to the next generation
                    nextGeneration.Add(child1);
                    nextGeneration.Add(child2);
                }

                // move to the next generation
                currentGeneration = nextGeneration;
                generationNumber++;
            }

            //Debug.WriteLine(shortestPathAllTime + " found at generation " + bestSolutionGenerationNumber);
            return bestSolution.Path;
        }

        private void CrossOverParents(CandidateSolution parent1, CandidateSolution parent2, 
            out CandidateSolution child1, out CandidateSolution child2)
        {
            child1 = parent1.DeepClone();
            child2 = parent2.DeepClone();

            // Do we use exact copies of parents, or crossover?
            if (Randomizer.GetDoubleFromZeroToOne() < crossoverRate)
            {
                // determine start and end crossover points
                int numCities = parent1.NumCities - 1;  
                int start = Randomizer.IntLessThan(numCities - 2); 
                int remaining = numCities - start;
                int end = Randomizer.IntBetween(numCities - remaining + 1, numCities - 1);

                for (int i=start; i <= end; i++)
                {
                    // the two values here tell us how to swap within each child
                    int child1cityIndex = child1.Path[i];
                    int child2cityIndex = child2.Path[i];
                    if (child1cityIndex == child2cityIndex)
                        continue;

                    // get references to both children's paths
                    var c1List = child1.Path.ToList();
                    var c2List = child2.Path.ToList();

                    // find within child1 and swap
                    int firstAt = c1List.IndexOf(child1cityIndex);
                    int secondAt = c1List.IndexOf(child2cityIndex);
                    int temp = c1List[firstAt];
                    c1List[firstAt] = c1List[secondAt];
                    c1List[secondAt] = temp;

                    // find within child2 and swap
                    firstAt = c2List.IndexOf(child1cityIndex);
                    secondAt = c2List.IndexOf(child2cityIndex);
                    temp = c2List[firstAt];
                    c2List[firstAt] = c2List[secondAt];
                    c2List[secondAt] = temp;
                }

                child1.CheckValidity();
                child2.CheckValidity();
            }
        }

        private CandidateSolution SelectCandidate()
        {
            // using Roulette Wheel Selection, we grab a possibility proportionate to it's fitness compared to
            // the total fitnesses of all possibilities
            int randomValue = Randomizer.IntLessThan(totalFitnessThisGeneration);
            for (int i = 0; i < populationSize; i++)
            {
                randomValue -= currentGeneration[i].Fitness;
                if (randomValue <= 0)
                {
                    return currentGeneration[i];
                }
            }

            return currentGeneration[populationSize - 1];  // ran off the end, give them the worst
        }

        private CandidateSolution SelectCandidateViaTournament()
        {
            // pick N random candidates, and then return the one with the highest fitness
            int bestFitness = int.MinValue;
            CandidateSolution bestFound = null;
            for (int i =0; i < tourneySize; i++)
            {
                int randomIndex = Randomizer.IntLessThan(populationSize);
                CandidateSolution randomSolution = currentGeneration[randomIndex];
                int fitness = randomSolution.Fitness;

                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    bestFound = randomSolution;
                }
            }

            return bestFound;
        }
    }
}
