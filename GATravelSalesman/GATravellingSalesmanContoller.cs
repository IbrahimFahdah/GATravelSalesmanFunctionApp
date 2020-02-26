using System;
using System.Collections.Generic;
using System.Diagnostics;
using Travelling_Salesman;

namespace GATravellingSalesman
{
    public partial class GATravellingSalesmanContoller
    {

        public CityCollection Cities = new CityCollection();

        public int NumCities { get; set; }

        public int ActualWidth, ActualHeight;

        public int PopulationSize { get; set; }
        public int CrossoverPercentage { get; set; }
        public double MutationPercentage { get; set; }
        public int NumIterations { get; set; }

      

        public GATravellingSalesmanContoller()
        {
            
        }

        public void CreateRandom()
        {
            const int Buffer = 10;
            //int numCities = Convert.ToInt32(txtNumCities.Text);
            int maxX = (int)ActualWidth - Buffer;
            int maxY = (int)ActualHeight - Buffer;

            Cities.Clear();
            for (int i = 0; i < NumCities; i++)
            {
                int x = Randomizer.IntLessThan(maxX);
                int y = Randomizer.IntLessThan(maxY);
                Cities.AddCity(x, y);
            }

        }


        public void CreateCircle()
        {
            int maxX = (int)ActualWidth ;
            int maxY = (int)ActualHeight ;

            int radius = (int)((double)Math.Min(maxX, maxY) / 2.2);
            int centerX = maxX / 2;
            int centerY = maxY / 2;
            double radiansBetween = (2 * Math.PI) / NumCities;
            double angle = Math.PI / -2;

            Cities.Clear();
            for (int i = 0; i < NumCities; i++)
            {
                int x = centerX + (int)(Math.Cos(angle) * radius);
                int y = centerY + (int)(Math.Sin(angle) * radius);
                Cities.AddCity(x, y);

                angle += radiansBetween;
            }

        }

        public PathChosen SolveTrad()
        {
            if (Cities.Count == 0) return null;

            PathChosen path = NonGASolution.NearestNeighbor(Cities);
            return path;
        }

        public PathChosen  SolveGA()
        {
            if (Cities.Count == 0) return null;

            //// clear old paths off the screen
            //cities.Draw(canvas);
            //gaResultTB.Text = "Calculating...";

            //int populationSize = (int)populationSizeSlider.Value;
            //int crossoverPercentage = (int)crossoverPctSlider.Value;
            //double mutationPercentage = Convert.ToDouble(txtMutationPct.Text);
            //int numIterations = Convert.ToInt32(txtNumIterations.Text);

            // Doing the GA takes a while, so kick off a thread for it
            return AsyncGACall(NumIterations, PopulationSize, CrossoverPercentage, MutationPercentage);
        }

        private PathChosen AsyncGACall(int numIterations, int populationSize, int crossoverPercentage, double mutationPercentage)
        {
            PathChosen bestPath = null;
            int shortestLength = int.MaxValue;
            int totalLength = 0;
            DateTime timeStart = DateTime.Now;

            for (int i = 0; i < numIterations; i++)
            {
                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    gaResultTB.Text = "Calculating #" + (i + 1) + "...";
                //}),
                //DispatcherPriority.Background);

                // do this long-running process in a non-UI thread
                int elitism = 10;
                int tourneySize = 2;
                var path = new GeneticAlgorithmEngine(populationSize, crossoverPercentage,
                    mutationPercentage, elitism, tourneySize).FindOptimalPath(Cities);
                int pathLength = path.GetTotalLength();
                totalLength += pathLength;

                if (pathLength < shortestLength)
                {
                    shortestLength = pathLength;
                    bestPath = new PathChosen(path);    // copy constructor
                }
            }

            var elapsed = DateTime.Now.Subtract(timeStart).TotalSeconds;
            Debug.WriteLine(numIterations + " iterations took " + elapsed + " seconds ");
            bestPath.AvLength = (int)(totalLength / numIterations);
            //// tell the UI thread to update the results, and display the best one 
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    if (gaDrawnPath != null)
            //        canvas.Children.Remove(gaDrawnPath);

            //    gaDrawnPath = bestPath.Draw(canvas, Brushes.Green);
            //    gaResultTB.Text = "Avg. path length: " + (int)(totalLength / numIterations) + " \nBest path length: " + shortestLength;
            //}),
            //    DispatcherPriority.Background);
            return bestPath;
        }


        }
}
