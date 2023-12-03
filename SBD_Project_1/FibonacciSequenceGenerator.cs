using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    public static class FibonacciSequenceGenerator
    {
        /// <summary>
        /// Generates sequence of n fibonacci numbers
        /// </summary>
        /// <param name="n">Length of sequence</param>
        /// <returns>Sequence of fibomacci numbers</returns>
        public static int[] Generate(int n)
        {
            int[] sequence = new int[n];
            sequence[0] = 1;
            sequence[1] = 1;
            for (int i = 2; i < n; i++)
            {
                sequence[i] = sequence[i - 1] + sequence[i - 2];
            }
            return sequence;
        }

        /// <summary>
        /// Generates n-th fibonacci number
        /// </summary>
        /// <param name="n">Index of number in fibonacci sequence</param>
        /// <returns>Fibonacci number</returns>
        public static long Get(int n)
        {
            if (n == 0 || n == 1)
                return 1;

            long a = 1;
            long b = 1;
            for (int i = 2; i < n; i++)
            {
                long temp = a;
                a = b;
                b = temp + b;
            }
            return b;
        }

        /// <summary>
        /// Generates part of sequence of n fibonacci numbers that sum is greater or equal m
        /// </summary>
        /// <param name="n">Length of sequence</param>
        /// <param name="m">Minimum sum of numbers in sequence</param>
        /// <returns>Sequence of n fibonacci numbers that sum is greater or equal m</returns>
        public static int[] GenerateDistribution(int n, int m)
        {
            Queue<int> sequence = new Queue<int>();
            sequence.Enqueue(1);
            sequence.Enqueue(1);
            while(true)
            {
                if(sequence.Count < n)
                {
                    sequence.Enqueue(sequence.TakeLast(2).Sum());
                }
                else
                {
                    if(sequence.Sum() >= m)
                        return sequence.ToArray();
                    else
                    {
                        sequence.Enqueue(sequence.TakeLast(2).Sum());
                        sequence.Dequeue();
                    }
                }
            }
        }
    }
}
