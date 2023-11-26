using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.Models
{
    public class Record
    {
        public virtual int MaxRecordLength { get; set; }

        public virtual int Index { get; set; }

        public Record() { }

        public virtual int[] GetContent()
        {
            throw new NotImplementedException();
        }

    }
}
