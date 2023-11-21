using SBD_Project_1.Generation;
using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.RecordGeneration
{
    internal class RecordFilesGenerator
    {
        private readonly RecordFile _file;

        public RecordFilesGenerator(RecordFile file)
        {
            _file = file;
        }
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
    }
}
