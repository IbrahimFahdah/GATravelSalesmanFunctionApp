namespace GATravelSalesmanFunctionApp
{
    internal class GATravellingSalesmanInput
    {
        public int NumCities { get; set; } = 20;

        public int PopulationSize { get; set; } = 500;

        public int CrossoverPercentage { get; set; } = 50;

        public double MutationPercentage { get; set; } = 2;

        public int NumIterations { get; set; } = 10;
    }
}
