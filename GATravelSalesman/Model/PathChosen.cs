using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace Travelling_Salesman
{
    public class PathChosen
    {
        public List<int> CityIndexes;
        public CityCollection Cities { get; }

        public int AvLength { get; set; }
        public int Length
        {
            get
            {
                return GetTotalLength();
            }
        } 
        public PathChosen(CityCollection setOfCities)
        {
            // keep a reference to the cities
            Cities = setOfCities;

            // and a corresponding list of indexes that specifies the path to take
            CityIndexes = new List<int>();
        }

        public PathChosen(PathChosen path)
        {
            // keep a reference to the cities
            Cities = path.Cities;

            // and copy an existing list of indexes that specifies the path to take
            CityIndexes = new List<int>(path.CityIndexes);
        }

        public void AddNextCity(City city)
        {
            int index = Cities.IndexOf(city);
            CityIndexes.Add(index);
        }

        public void CreateRandomOrdering()
        {
            int numCities = Cities.Count - 1;

            // initialize with all values from 1 to end
            for (int i=1; i <= numCities; i++)
            {
                CityIndexes.Add(i);
            }

            // then shuffle them up
            for (int pass=0; pass < 5; pass++)
                for (int i=0; i < CityIndexes.Count; i++)
                {
                    // swap randomly
                    int j = Randomizer.IntLessThan(CityIndexes.Count);

                    int temp = CityIndexes[j];
                    CityIndexes[j] = CityIndexes[i];
                    CityIndexes[i] = temp;
                }
        }

        public int GetTotalLength()
        {
            int total = 0;
            City firstCity = Cities[0];
            int lastX = firstCity.X;
            int lastY = firstCity.Y;

            for(int i = 0; i < CityIndexes.Count; i++)
            {
                var city = Cities[CityIndexes[i]];
                total += Geometry.GetDistance(city.X, city.Y, lastX, lastY);

                lastX = city.X;
                lastY = city.Y;
            }
            total += Geometry.GetDistance(lastX, lastY, firstCity.X, firstCity.Y);

            return total;
        }

        public void InsertAt(int newPosition, int cityIndex)
        {
            CityIndexes.Insert(newPosition, cityIndex);
        }

        public void RemoveAt(int which)
        {
            CityIndexes.RemoveAt(which);
        }

        public int this[int index]
        {
            get { return this.CityIndexes[index]; }
            set { CityIndexes[index] = value; }
        }

        public List<int> ToList()
        {
            return CityIndexes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i < CityIndexes.Count; i++)
            {
                sb.Append(this.CityIndexes[i].ToString() + "-");
            }
            return sb.ToString();
        }

        //public void Draw(System.Drawing.ima.Bitmap canvas)
        //{
        //    //City firstCity = Cities[0];

        //    //PathGeometry geometry = new PathGeometry();
        //    //PathFigure figure = new PathFigure();
        //    //geometry.Figures.Add(figure);

        //    //figure.StartPoint = new System.Windows.Point(firstCity.X, firstCity.Y);
        //    //figure.IsClosed = true;

        //    //// create a line segment for each leg in the path
        //    //for (int i = 0; i < this.cityIndexes.Count; i++)
        //    //{
        //    //    City city = Cities[cityIndexes[i]];

        //    //    LineSegment segment = new LineSegment();
        //    //    segment.Point = new System.Windows.Point(city.X, city.Y);
        //    //    figure.Segments.Add(segment);
        //    //}

        //    //// add a single Path object to the canvas
        //    //var path = new Path();
        //    //path.Stroke = lineBrush;
        //    //path.StrokeThickness = 2;
        //    //path.Data = geometry;

        //    //// add the whole thing as a single Path
        //    //canvas.Children.Add(path);

        //    //return path;
        //}
    }
}
