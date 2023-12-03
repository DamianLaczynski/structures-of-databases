using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    public static class PrimeNumbersCounter
    {
        /// <summary>
        /// Counts prime numbers in table
        /// </summary>
        /// <param name="table">Table of numbers</param>
        /// <returns>Number of primes in table</returns>
        public static int Count(int[] table)
        {
            int counter = 0;
            foreach (int number in table)
            {
                if (IsPrime(number))
                {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Checks if number is prime
        /// </summary>
        /// <param name="number">Number to check</param>
        /// <returns>True if number is prime</returns>
        public static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            var boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = 3; i <= boundary; i += 2) { if (number % i == 0) return false; }
            return true;
        }
    }
}
