﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBD_Project_1.PoliphaseSort;

namespace SBD_Project_1.Models
{
    internal class NaturalNumbersSetRecord : Record
    {
        private int[] _numbers { get; set; } = new int[Configuration.MAX_RECORD_LENGTH];

        public NaturalNumbersSetRecord(int[] numbers)
        {
            if (numbers.Length > Configuration.MAX_RECORD_LENGTH)
            {
                throw new ArgumentException($"Record can't have more than {Configuration.MAX_RECORD_LENGTH} elements");
            }
            else if(numbers.Length == 0 || numbers[0] == 0)
            {
                throw new ArgumentException($"Record can't be empty");
            }
            else if(numbers.Length < Configuration.MAX_RECORD_LENGTH)
            {
                _numbers = new int[Configuration.MAX_RECORD_LENGTH];
                for (int i = 0; i < numbers.Length; i++)
                {
                    _numbers[i] = numbers[i];
                }
                Index = PrimeNumbersCounter.Count(_numbers);
            }
            else
            {
                _numbers = numbers;
                Index = PrimeNumbersCounter.Count(numbers);
            }
        }

        public NaturalNumbersSetRecord(byte[] numbers)
        {
            if (numbers.Length/sizeof(int) > Configuration.MAX_RECORD_LENGTH)
            {
                throw new ArgumentException($"Record can't have more than {Configuration.MAX_RECORD_LENGTH} elements");
            }
            else if (numbers.Length == 0 || numbers[0] == 0)
            {
                throw new ArgumentException($"Record can't be empty");
            }
            else
            {
                _numbers = ArrayConverter.ToIntArray(numbers);
                Index = PrimeNumbersCounter.Count(_numbers);
            }
        }

        public override int[] GetContent()
        {
            return _numbers;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string result = "Index:" + Index + "| ";
            foreach (int number in _numbers)
            {
                result += number + " ";
            }
            return result;
        }

    }
}
