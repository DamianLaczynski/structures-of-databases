using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
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
    }
}
