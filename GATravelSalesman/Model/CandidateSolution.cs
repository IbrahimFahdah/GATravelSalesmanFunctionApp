using System.Collections.Generic;
using System.Diagnostics;

namespace Travelling_Salesman
{
    class CandidateSolution
    {
        private PathChosen path;
        private readonly CityCollection cities;
        private int pathLength;

        public CandidateSolution(CityCollection availableCities)
        {
            cities = availableCities;
            path = new PathChosen(cities);
            path.CreateRandomOrdering();
        }

        public CandidateSolution(PathChosen existingPath)
        {
            path = new PathChosen(existingPath);
            cities = path.Cities;
        }

        public int Fitness { get; set; }

        public int PathLength
        {
            get
            {
                return pathLength;
            }
        }

        public void CalculatePathLength()
        {
            pathLength = path.GetTotalLength();
        }

        public int NumCities
        {
            get
            {
                return cities.Count;
            }
        }

        public void CheckValidity()
        {
            List<int> used = new List<int>();
            for (int i = 0; i < path.Cities.Count - 1; i++)
            {
                int val = path[i];

                if (used.Contains(val))
                {
                    Debug.Assert(false);
                }
                else
                    used.Add(val);
            }
        }

        public void DoSwapMutation(double mutationRate)
        {
            if (Randomizer.GetDoubleFromZeroToOne() < mutationRate)
            {
                // simply swap two randomly selected cities
                int first, second;
                do
                {
                    first = Randomizer.IntLessThan(NumCities - 1);
                    second = Randomizer.IntLessThan(NumCities - 1);
                }
                while (first == second);

                int temp = path[first];
                path[first] = path[second];
                path[second] = temp;

                CheckValidity();
            }
        }

        public void DoDisplacementMutation(double mutationRate)
        {
            if (Randomizer.GetDoubleFromZeroToOne() < mutationRate)
            {
                int which = Randomizer.IntLessThan(NumCities - 1);
                int cityIndex = path[which];
                path.RemoveAt(which);

                int newPosition = Randomizer.IntLessThan(NumCities - 1);    
                path.InsertAt(newPosition, cityIndex);

                CheckValidity();
            }
        }

        public PathChosen Path
        {
            get
            {
                return this.path;
            }
        }

        public CandidateSolution DeepClone()
        {
            return new CandidateSolution(path);
        }

    }
}
