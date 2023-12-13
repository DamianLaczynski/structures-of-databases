using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.PoliphaseSort
{
    /// <summary>
    /// Compares records by index
    /// Sorts in ascending order
    /// </summary>
    internal class MorePrimeNumbersFirstComparer : IComparer<Record>
    {

        int IComparer<Record>.Compare(Record? x, Record? y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException();
            }
            if (x.Index > y.Index)
            {
                return 1;
            }
            else if (x.Index < y.Index)
            {
                return -1;
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
