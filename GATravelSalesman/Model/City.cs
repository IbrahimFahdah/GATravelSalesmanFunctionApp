namespace Travelling_Salesman
{
    public class City
    {
        public int X { get; set; }
        public int Y { get; set; }

        public const int ClickRadius = 12;

        public bool WithinClickRadius(int x, int y)
        {
            return Geometry.GetDistance(x, y, this.X, this.Y) <= ClickRadius;
        }
    }
}
