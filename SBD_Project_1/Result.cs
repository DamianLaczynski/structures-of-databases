using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    enum ResultType
    {
        Success,
        Failure,
        AlreadyExists,
    }
    internal class Result
    {
        public Result()
        {
            Type = ResultType.Success;
            Reads = 0;
            Writes = 0;
        }
        public Result(int reads, int writes)
        {
            Type = ResultType.Success;
            Reads = reads;
            Writes = writes;
        }
        public Result(int reads, int writes, ResultType result)
        {
            Type = result;
            Reads = reads;
            Writes = writes;
        }
        public ResultType Type { get; set; }
        public int Reads { get; set; }
        public int Writes { get; set; }

        public Record Record { get; set; }

        public override string ToString()
        {
            return $"Result: {Type}, Reads: {Reads}, Writes: {Writes}";
        }
    }
}
