using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.PoliphaseSort
{
    internal static class SeriesSetter
    {
        /// <summary>
        /// Moves series of records from source tape to destination tape
        /// </summary>
        /// <param name="source">Source tape</param>
        /// <param name="destination"> Destination tape</param>
        /// <returns>Last record on serie</returns>
        public static Record SetSerie(Tape source, Tape destination)
        {
            Record last = null;
            while (true)
            {
                Record record = source.GetRecord();
                if (record is null)
                {
                    destination.EmptySeriesCount++;

                    break;
                }
                last = record;
                destination.SetRecord(record);
                //check end of serie
                Record nextRecord = source.GetNextRecord();
                if (nextRecord is null)
                {
                    destination.SeriesCount++;

                    break;
                }
                else if (record.Index > nextRecord.Index)
                {
                    destination.SeriesCount++;

                    break;
                }
            }
            return last;
        }

        /// <summary>
        /// Sets and merges records from sources tapes to destination tape
        /// </summary>
        /// <param name="sources">Sources tapes</param>
        /// <param name="destination">Destination tape</param>
        public static void SetAndMerge(Tape[] sources, Tape destination, IComparer<Record> comparer)
        {
            Record[] records = new Record[Configuration.TAPES_COUNT - 1];
            bool[] isEndOfSeries = new bool[Configuration.TAPES_COUNT - 1];
            for (int i = 0; i < Configuration.TAPES_COUNT - 1; i++)
            {
                records[i] = null;
                isEndOfSeries[i] = false;
            }
            while (!isEndOfSeries.All(b => b == true))
            {
                for (int i = 0; i < Configuration.TAPES_COUNT - 1; i++)
                {
                    if (records[i] is null && !isEndOfSeries[i])
                    {
                        records[i] = sources[i].GetRecord();
                        if (records[i] is null)
                        {
                            isEndOfSeries[i] = true;
                            sources[i].SeriesCount--;
                        }
                    }
                }
                //find min and set to writting tape
                var temp = records.ToList().Min(comparer);
                for (int i = 0; i < Configuration.TAPES_COUNT - 1; i++)
                {
                    if (records[i] is not null && records[i].Equals(temp))
                    {
                        //Console.WriteLine($"Move to {destination.GetName()}:{records[i]}");
                        destination.SetRecord(records[i]);
                        if (sources[i].GetNextRecord() is null || sources[i].GetNextRecord().Index < records[i].Index)
                        {
                            isEndOfSeries[i] = true;
                            sources[i].SeriesCount--;
                        }
                        records[i] = null;
                    }
                }
            }
            destination.SeriesCount++;
        }
    }
}
