using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBD_Project_1.Models;

namespace SBD_Project_1.Generation
{
    internal class RandomRecordGenerator : RecordGenerator
    {
        private readonly Random _random = new Random();
        private readonly int _maxRecordLength = int.Parse(Configuration.configuration["maxRecordLength"]);
        private readonly int _maxNumber = int.Parse(Configuration.configuration["maxNumberInRecord"]);

        public override NaturalNumbersSetRecord GetRecord()
        {
            int[] numbers = new int[_maxRecordLength];
            foreach (int number in numbers)
            {
                numbers[number] = 0;
            }
            for (int i = 0; i < _random.Next(1, _maxRecordLength); i++)
            {
                numbers[i] = _random.Next(1, _maxNumber);
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
