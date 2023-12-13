using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.Models
{
    public static class ArrayConverter
    {
        /// <summary>
        /// Converts int array to byte array
        /// </summary>
        /// <param name="intArray"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(int[] intArray)
        {
            byte[] byteArray = new byte[intArray.Length * sizeof(int)];

            for (int i = 0; i < intArray.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(intArray[i]);
                Buffer.BlockCopy(temp, 0, byteArray, i * sizeof(int), sizeof(int));
            }

            return byteArray;
        }
        /// <summary>
        /// Converts queue of records to byte array
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(Queue<Record> queue)
        {
            byte[] byteArray = new byte[queue.Count * 15 * sizeof(int)];
            int desOffset = 0;
            foreach(Record record in queue)
            {
                byte[] temp = ToByteArray(record.GetContent());
                Buffer.BlockCopy(temp, 0, byteArray, desOffset, temp.Length);
                desOffset += temp.Length;
            }
            return byteArray;
        }

        /// <summary>
        /// Converts byte array to int array
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static int[] ToIntArray(byte[] byteArray)
        {
            int[] intArray = new int[byteArray.Length / sizeof(int)];

            for(int i=0; i < byteArray.Length; i += sizeof(int))
            {
                intArray[i / sizeof(int)] = BitConverter.ToInt32(byteArray, i);
            }
            return intArray;
        }

        /// <summary>
        /// Converts byte array to queue of records
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static Queue<Record> ToRecordQueue(byte[] byteArray)
        {
            Queue<Record> recordQueue = new Queue<Record>();
            int[] intArray = ArrayConverter.ToIntArray(byteArray);
            int[] record = new int[Configuration.MAX_RECORD_LENGTH];
            for (int i = 0; i < intArray.Length; i++)
            {

                record[i % Configuration.MAX_RECORD_LENGTH] = intArray[i];
                if (i % Configuration.MAX_RECORD_LENGTH == Configuration.MAX_RECORD_LENGTH-1)
                {
                    recordQueue.Enqueue((Record)new NaturalNumbersSetRecord(record));
                    record = new int[Configuration.MAX_RECORD_LENGTH];
                }
            }
            return recordQueue;
        }

        /// <summary>
        /// Converts byte array to list of records
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static List<Record> ToRecordList(byte[] byteArray)
        {
            List<Record> recordList = new List<Record>();
            int[] intArray = ArrayConverter.ToIntArray(byteArray);
            int[] record = new int[Configuration.MAX_RECORD_LENGTH];
            for (int i = 0; i < intArray.Length; i++)
            {

                record[i % Configuration.MAX_RECORD_LENGTH] = intArray[i];
                if (i % Configuration.MAX_RECORD_LENGTH == Configuration.MAX_RECORD_LENGTH-1)
                {
                    recordList.Add((Record)new NaturalNumbersSetRecord(record));
                    record = new int[Configuration.MAX_RECORD_LENGTH];
                }
            }
            return recordList;
        }

    }
}
