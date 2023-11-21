using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using SBD_Project_1.Models;

namespace SBD_Project_1.Generation
{
    internal class RecordGenerator 
    {
        public virtual Record GetRecord()
        {
            //throw new NotImplementedException();
            return new Record();
        }

        public virtual List<Record> GetRecords(int count)
        {
            //throw new NotImplementedException();
            return new List<Record>();
        }
    }
}
