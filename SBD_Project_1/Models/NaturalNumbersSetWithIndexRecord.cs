using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.Models
{
    internal class NaturalNumbersSetWithIndexRecord : Record
    {
        private Record _data { get; set; }

        

        public NaturalNumbersSetWithIndexRecord(int key, int overflowPointer, Record data)
        {
            _data = data;
            OverflowPointer = overflowPointer;
            Key = key;
        }
        public NaturalNumbersSetWithIndexRecord(int key, Record data)
        {
            _data = data;
            OverflowPointer = -1;
            Key = key;
        }
        public NaturalNumbersSetWithIndexRecord(int[] content)
        {
            Key = content[0];
            OverflowPointer = content[1];
            int[] data = new int[content.Length - 2];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = content[i + 2];
            }
            _data = new NaturalNumbersSetRecord(data);
        }

        public override void Update(Record record)
        {
            OverflowPointer = record.OverflowPointer;
            _data = record;
        }

        public override int[] GetContent()
        {
            var data = _data.GetContent();
            //structure of content: [key,overflowPointer, data]
            int[] content = new int[data.Length + 2];
            content[0] = Key;
            content[1] = OverflowPointer;
            for (int i = 0; i < data.Length; i++)
            {

                content[i + 2] = data[i];
            }
            return content;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            string result = "Key:" + Key + "| " + "OP:" + OverflowPointer + "| ";
            foreach (int number in _data.GetContent())
            {
                result += number + " ";
            }
            return result;
        }
    }
}
