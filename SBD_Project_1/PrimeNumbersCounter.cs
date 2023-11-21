using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    public static class PrimeNumbersCounter
    {
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
