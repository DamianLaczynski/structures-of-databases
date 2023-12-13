using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.PoliphaseSort
{
    internal class FibonacciDistibution
    {
        /// <summary>
        /// Distributes records from source tape to destination tapes
        /// </summary>
        /// <param name="source">Source tape</param>
        /// <param name="destinations">Destination tapes</param>
        /// <returns>Number of runs(series)</returns>
        public static long Distribute(Tape source, List<Tape> destinations)
        {
            long runsCount = 0;
            Record[] lastOnTape = new Record[Configuration.TAPES_COUNT - 1];
            for (int j = 0; j < Configuration.TAPES_COUNT - 1; j++)
            {
                lastOnTape[j] = null;
            }
            int i = 0;
            while (!source.IsEmpty())
            {
                for (int j = 0; j < FibonacciSequenceGenerator.Get(i); j++)
                {
                    //preventing series from combining in distributing phase
                    if (lastOnTape[i % (Configuration.TAPES_COUNT - 1)] is not null &&
                        source.GetNextRecord() is not null &&
                        lastOnTape[i % (Configuration.TAPES_COUNT - 1)].Index < source.GetNextRecord().Index)
                    {
                        lastOnTape[i % (Configuration.TAPES_COUNT - 1)] = SeriesSetter.SetSerie(source, destinations[i % (Configuration.TAPES_COUNT - 1)]);
                        destinations[i % (Configuration.TAPES_COUNT - 1)].SeriesCount--;
                        runsCount--;
                    }
                    lastOnTape[i % (Configuration.TAPES_COUNT - 1)] = SeriesSetter.SetSerie(source, destinations[i % (Configuration.TAPES_COUNT - 1)]);
                    runsCount++;
                }
                i++;
            }
            return runsCount;
        }
    }
}
