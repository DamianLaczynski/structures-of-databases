using SBD_Project_1.Generation;
using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.RecordGeneration
{
    /// <summary>
    /// Class for generating records and writing them to file
    /// </summary>
    internal class RecordFilesGenerator
    {
        private readonly RecordFile _file;

        public RecordFilesGenerator(RecordFile file)
        {
            _file = file;
        }
        /// <summary>
        /// Generates records and writes them to file
        /// </summary>
        /// <param name="recordsCout"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void Generate(int recordsCout)
        {
            if (_file != null)
            {
                RecordGenerator recordGenerator = new RandomRecordGenerator();
                var records = recordGenerator.GetRecords(recordsCout);
                _file.Write(records);
            }
            else
            {
                throw new NullReferenceException("File is null");
            }

        }
        /// <summary>
        /// Generates records from user input and writes them to file
        /// </summary>
        /// <param name="recordsCout">Number of records</param>
        public void EnterRecords(int recordsCout)
        {
            RecordGenerator recordGenerator = new ManualInputRecordGenerator();
            var records = recordGenerator.GetRecords(recordsCout);
            _file.Write(records);
        }
    }
}
