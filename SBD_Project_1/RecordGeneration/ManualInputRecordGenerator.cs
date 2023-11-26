using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBD_Project_1.Models;

namespace SBD_Project_1.Generation
{
    internal class ManualInputRecordGenerator : RecordGenerator
    {
        //gets record from user input
        public override NaturalNumbersSetRecord GetRecord()
        {
            int[] numbers;
            string[] inputArray;
            bool isIncorrectInput = true;

            do {
                Console.WriteLine("Enter record elements separated by space");
                string input = Console.ReadLine();
                inputArray = input.Split(' ');
                numbers = new int[inputArray.Length];
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
