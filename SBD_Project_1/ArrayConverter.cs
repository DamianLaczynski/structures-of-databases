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
        public static byte[] ToByteArray(int[] intArray)
        {
            byte[] byteArray = new byte[intArray.Length * sizeof(int)];

            for (int i = 0; i < intArray.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(intArray[i]);
                Buffer.BlockCopy(temp, 0, byteArray, i * sizeof(int), sizeof(int));
            }

            Buffer.BlockCopy(intArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }

        public static int[] ToIntArray(byte[] byteArray)
        {
            int[] intArray = new int[byteArray.Length / sizeof(int)];

            for(int i=0; i < byteArray.Length; i += sizeof(int))
            {
                intArray[i / sizeof(int)] = BitConverter.ToInt32(byteArray, i);
            }
            return intArray;
        }

        public static Queue<Record> ToRecordQueue(byte[] byteArray)
        {
            Queue<Record> recordList = new Queue<Record>();
            int[] intArray = ArrayConverter.ToIntArray(byteArray);
            int[] record = new int[NaturalNumbersSetRecord.MaxRecordLength];
            for (int i = 0; i < intArray.Length; i++)
            {

                record[i % NaturalNumbersSetRecord.MaxRecordLength] = intArray[i];
                if (i % NaturalNumbersSetRecord.MaxRecordLength == NaturalNumbersSetRecord.MaxRecordLength-1)
                {
                    //Console.WriteLine($"{i/NaturalNumbersSetRecord.MaxRecordLength}:{new NaturalNumbersSetRecord(record)}");
                    recordList.Enqueue((Record)new NaturalNumbersSetRecord(record));
                    record = new int[NaturalNumbersSetRecord.MaxRecordLength];
                }
            }
            return recordList;
        }
        public static List<Record> ToRecordList(byte[] byteArray)
        {
            List<Record> recordList = new List<Record>();
            int[] intArray = ArrayConverter.ToIntArray(byteArray);
            int[] record = new int[NaturalNumbersSetRecord.MaxRecordLength];
            for (int i = 0; i < intArray.Length; i++)
            {

                record[i % NaturalNumbersSetRecord.MaxRecordLength] = intArray[i];
                if (i % NaturalNumbersSetRecord.MaxRecordLength == NaturalNumbersSetRecord.MaxRecordLength-1)
                {
                    //Console.WriteLine($"{i/NaturalNumbersSetRecord.MaxRecordLength}:{new NaturalNumbersSetRecord(record)}");
                    recordList.Add((Record)new NaturalNumbersSetRecord(record));
                    record = new int[NaturalNumbersSetRecord.MaxRecordLength];
                }
            }
            return recordList;
        }

    }
}
