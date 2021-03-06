﻿using System;

namespace Travelling_Salesman
{
    class Randomizer
    {
        private static Random randomizer = new Random();

        /// <summary>
        /// Inclusive, random value returned may include either upper or lower boundary
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int IntBetween(int lower, int upper)
        {
            return randomizer.Next(lower, upper);
        }

        /// <summary>
        /// Returns an int up to, but not including the parameter
        /// </summary>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int IntLessThan(int upper)
        {
            return randomizer.Next(upper);
        }

        /// <summary>
        /// Returns a double >= 0 and less than 1.0
        /// </summary>
        /// <returns></returns>
        public static double GetDoubleFromZeroToOne()
        {
            return randomizer.NextDouble();
        }
    }
}
