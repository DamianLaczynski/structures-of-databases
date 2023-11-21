﻿using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class MorePrimeNumbersFirstComparer : Comparer<NaturalNumbersSetRecord>
    {
        public override int Compare(NaturalNumbersSetRecord? x, NaturalNumbersSetRecord? y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException();
            }
            if (x.PrimeNumbersCount > y.PrimeNumbersCount)
            {
                return -1;
            }
            else if (x.PrimeNumbersCount < y.PrimeNumbersCount)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}