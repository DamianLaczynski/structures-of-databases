using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.Models
{
    public class Record
    {
        public int Index { get; set; }
        public int Key { get; set; }

        public int OverflowPointer { get; set; }

        public Record() { }

        public virtual void Update(Record record) 
        {
            throw new NotImplementedException();
        }

        public virtual int[] GetContent()
        {
            throw new NotImplementedException();
        }

    }
}
