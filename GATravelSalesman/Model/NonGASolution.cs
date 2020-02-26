using System.Collections.Generic;

namespace Travelling_Salesman
{
    public class NonGASolution
    {
        public static PathChosen NearestNeighbor(CityCollection cities)
        {
            PathChosen result = new PathChosen(cities);
            int numAvailable = cities.Count - 1;
            List<City> usedCities = new List<City>();   // initialize empty

            City currentCity = cities[0];
            usedCities.Add(currentCity);
            while (numAvailable > 0)
            {
                // find the next closest
                City closest = cities.NearestTo(currentCity.X, currentCity.Y, usedCities);

                // add to our path
                result.AddNextCity(closest);
                currentCity = closest;

                // remember that we've seen this
                usedCities.Add(closest);
                numAvailable--;
            }

            return result;
        }
    }
}
