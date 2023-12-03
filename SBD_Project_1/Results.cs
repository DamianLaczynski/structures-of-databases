using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    /// <summary>
    /// Contains results of sorting
    /// </summary>
    internal class Results
    {
        RecordFile _file;
        long _reads;
        long _writes;
        long _totalTime;
        long _phases;
        long _runs;

        public Results(RecordFile file, long reads, long writes, long phases, long runs)
        {
            _file = file;
            _reads = reads;
            _writes = writes;
            _phases=phases;
            _runs = runs;
        }

        public Results(RecordFile file, long reads, long writes, long totalTime, long phases, long runs)
        {
            _file = file;
            _reads = reads;
            _writes = writes;
            _totalTime=totalTime;
            _phases=phases;
            _runs = runs;
        }

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nResults");
            Console.ResetColor();

            //write statistics
            Console.WriteLine($"Records: {Configuration.RECORDS_COUNT}");
            Console.WriteLine($"Tapes: {Configuration.TAPES_COUNT}");
            Console.WriteLine($"Buffer Size: {Configuration.BUFFER_SIZE}");
            Console.WriteLine($"Reads: {_reads}");
            Console.WriteLine($"Writes: {_writes}");
            Console.WriteLine($"Phases: {_phases}");
            Console.WriteLine($"Runs: {_runs}");
            Console.WriteLine($"Disk access: {_writes+_reads}");
            Console.WriteLine($"Time: {_totalTime} milliseconds");

            //write file content
            _file.Print();
        }
    }
}
