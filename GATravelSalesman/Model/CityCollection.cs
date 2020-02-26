using System;
using System.Collections.Generic;


namespace Travelling_Salesman
{
    public class CityCollection
    {
        private List<City> cities;

        public CityCollection()
        {
            cities = new List<City>();
        }

        public void Clear()
        {
            cities.Clear();
        }

        public void AddCity(int x, int y)
        {
            var city = new City() { X = x, Y = y };
            cities.Add(city);
        }

        public void Remove(City city)
        {
            cities.Remove(city);
        }

        public int Count
        {
            get
            {
                return cities.Count;
            }
        }

        public int IndexOf(City city)
        {
            return cities.IndexOf(city);
        }

        public List<City> ToList()
        {
            return cities;
        }

        public City NearestTo(int x, int y, List<City> citiesToIgnore = null)
        {
            int closestDistance = Int32.MaxValue;
            City closest = null;
            foreach (var city in cities)
            {
                // for the non-GA solution, there's a list of already selected cities
                // so we ignore any in there
                if ((citiesToIgnore != null) && (citiesToIgnore.Contains(city)))
                    continue;

                int dist = Geometry.GetDistance(city.X, city.Y, x, y);
                if (dist < closestDistance)
                {
                    closest = city;
                    closestDistance = dist;
                }
            }
            return closest;
        }

        public City this[int index]
        {
            get { return cities[index]; }
            set { cities[index] = value; }
        }

        //public void Draw(Canvas canvas)
        //{
        //    // create a display element for each city
        //    canvas.Children.Clear();
        //    bool hasDrawnFirst = false;
        //    foreach (var city in cities)
        //    {
        //        var displayElement = new Ellipse();
        //        SolidColorBrush brush = new SolidColorBrush();
        //        if (hasDrawnFirst == false)
        //        {
        //            hasDrawnFirst = true;
        //            brush.Color = Color.FromRgb(255, 0, 0);
        //        }
        //        else
        //            brush.Color = Color.FromRgb(0, 255, 0);

        //        displayElement.Fill = brush;
        //        displayElement.StrokeThickness = 2;
        //        displayElement.Stroke = Brushes.Black;

        //        displayElement.Width = City.ClickRadius * 2;
        //        displayElement.Height = displayElement.Width;

        //        Canvas.SetTop(displayElement, city.Y - City.ClickRadius);
        //        Canvas.SetLeft(displayElement, city.X - City.ClickRadius);

        //        canvas.Children.Add(displayElement);                
        //    }
        //}
    }
}
