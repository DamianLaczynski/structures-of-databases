using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBD_Project_1.Models;

namespace SBD_Project_1.Generation
{
    /// <summary>
    /// Class for generating records from user input
    /// </summary>
    internal class ManualInputRecordGenerator : RecordGenerator
    {
        /// <summary>
        /// Gets record from user input
        /// </summary>
        /// <returns></returns>
        public override NaturalNumbersSetRecord GetRecord()
        {
            int[] numbers;
            string[] inputArray;
            bool isIncorrectInput = true;

            do {
                Console.WriteLine("Enter record elements separated by space");
                string input = Console.ReadLine();
                inputArray = input.Split(' ');
                numbers = new int[Configuration.MAX_RECORD_LENGTH];
                for(int i = 0; i < Configuration.MAX_RECORD_LENGTH; i++)
                {
                    numbers[i] = 0;
                }
                if (inputArray.Length > Configuration.MAX_RECORD_LENGTH)
                {
                    Console.WriteLine($"Record can't have more than {Configuration.MAX_RECORD_LENGTH} elements");
                    Console.WriteLine($"Please try again...");
                }
                else if(inputArray.Length == 0 || inputArray[0] == "")
                {
                    Console.WriteLine($"Record can't be empty");
                    Console.WriteLine($"Please try again...");
                }
                else
                {
                    isIncorrectInput = false;
                }
            } while (isIncorrectInput);

            for (int i = 0; i < inputArray.Length; i++)
            {
                numbers[i] = int.Parse(inputArray[i]);
            }
            return new NaturalNumbersSetRecord(numbers);
        }

        /// <summary>
        /// Gets list of records from user input
        /// </summary>
        /// <param name="count">Number of records</param>
        /// <returns>List of taken records </returns>
        public override List<Record> GetRecords(int count)
        {
            List<Record> records = new List<Record>();
            for (int i = 0; i < count; i++)
            {
                records.Add(GetRecord());
            }
            return records;
        }

    }
}
