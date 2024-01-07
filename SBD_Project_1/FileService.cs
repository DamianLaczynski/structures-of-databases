using SBD_Project_1.Models;
using SBD_Project_1.PoliphaseSort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SBD_Project_1
{
    internal class FileService
    {
        private IndexFile _indexFile;
        private PrimaryAreaFile _primaryAreaFile;
        private OverflowAreaFile _overflowAreaFile;
        public FileService()
        {
            _indexFile = new IndexFile();
            _primaryAreaFile = new PrimaryAreaFile();
            _overflowAreaFile = new OverflowAreaFile();
        }

        public Result InsertRecord(int key, Record record)
        {
            var result = new Result();
            var firstKey = GetFirstKey();
            if (firstKey != -1 && key < firstKey)
            {
                var firstRecord = _primaryAreaFile.GetRecord(0, firstKey);
                _primaryAreaFile.UpdateRecord(0, firstKey, key, record);
                result = InsertRecord(firstKey, firstRecord.GetData());
                _indexFile.UpdateRecord(firstKey, key);
                return result;
            }
            try
            {
                var pageNo = _indexFile.GetPageNo(key);
                var pointer = _primaryAreaFile.AddRecord(pageNo, new NaturalNumbersSetWithIndexRecord(key, record));
                if (pointer == -1)
                {
                    _overflowAreaFile.AddRecord(new NaturalNumbersSetWithIndexRecord(key, record));
                }
                else if (pointer > 0)
                {
                    _overflowAreaFile.AddRecord(pointer, new NaturalNumbersSetWithIndexRecord(key, record));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result.Type = ResultType.Failure;
            }
            finally
            {

            }
            CountDiscAccesses(ref result);

            if(_overflowAreaFile.EnableSpace <= 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reorganizing...");
                Console.ResetColor();
                Reorganize();
                DisplayFile();
            }

            //Debug();

            return result;
        }
        public Result UpdateRecord(int key, int newKey, Record record)
        {
            var result = new Result();
            if (key == newKey)
            {
                try
                {
                    var pageNo = _indexFile.GetPageNo(key);
                    _primaryAreaFile.UpdateRecord(pageNo, key, record);

                    //TODO: update overflow area

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {

                }
            }
            else
            {
                try
                {
                    var pageNo = _indexFile.GetPageNo(key);
                    DeleteRecord(key);
                    InsertRecord(newKey, record);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {

                }
            }
            CountDiscAccesses(ref result);

            return result;
        }
        public Result DeleteRecord(int key)
        {
            var result = new Result();
            var pageNo = _indexFile.GetPageNo(key);
            try
            {
                _primaryAreaFile.RemoveRecord(pageNo, key);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Key not found in prime area");
                try
                {
                    _overflowAreaFile.RemoveRecord(key);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message + "Key not found in overflow area");
                }
            }
            finally
            {

            }
            CountDiscAccesses(ref result);

            return result;
        }
        public Result GetRecord(int key)
        {
            var result = new Result();
            var pageNo = _indexFile.GetPageNo(key);
            try
            {
                var record = _primaryAreaFile.GetRecord(pageNo, key);
                result.Record = record;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Key not found in prime area");
                try
                {
                    result.Record = _overflowAreaFile.GetRecord(key);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message + "Key not found in overflow area");
                }
            }
            finally
            {

            }

            CountDiscAccesses(ref result);

            return result;

        }

        private void CountDiscAccesses(ref Result result)
        {
            result.Reads = _indexFile.Reads + _primaryAreaFile.Reads + _overflowAreaFile.Reads;
            result.Writes = _indexFile.Writes + _primaryAreaFile.Writes + _overflowAreaFile.Writes;

            _indexFile.ResetOperationCount();
            _primaryAreaFile.ResetOperationCount();
            _overflowAreaFile.ResetOperationCount();
        }
        public Result DisplayFile()
        {
            _indexFile.Display();
            _primaryAreaFile.Display();
            _overflowAreaFile.Display();

            return new Result();
        }
        public Result Reorganize()
        {
            PrepareNewFiles(out IndexFile indexFile, out PrimaryAreaFile primaryAreaFile, out OverflowAreaFile overflowAreaFile);

            //while not all records from the old file have beed processed
            while ((_primaryAreaFile.RecordsCount + _overflowAreaFile.RecordsCount) > 0)
            {
                var record = _primaryAreaFile.GetNextRecord();

                //ignore deleted records
                if (record.Key == 0)
                {
                    continue;
                }

                var overflowPointer = record.OverflowPointer;
                record.OverflowPointer = -1;

                primaryAreaFile.AddRecord(record, ref indexFile);
                while (overflowPointer != -1)
                {
                    record = _overflowAreaFile.GetNextRecord(overflowPointer);

                    overflowPointer = record.OverflowPointer;
                    record.OverflowPointer = -1;

                    primaryAreaFile.AddRecord(record, ref indexFile);

                }
            }
            _indexFile = indexFile;
            _primaryAreaFile = primaryAreaFile;
            _overflowAreaFile = overflowAreaFile;

            //Delete the old file and the old index.
            //TODO

            return new Result();
        }

        private void PrepareNewFiles(out IndexFile indexFile, out PrimaryAreaFile primaryAreaFile, out OverflowAreaFile overflowAreaFile)
        {
            _primaryAreaFile.InitReorganization();

            var primaryPages = Math.Ceiling((_primaryAreaFile.RecordsCount+_overflowAreaFile.RecordsCount)/(Configuration.FileOrganization.PrimaryAreaRecordsCount*Configuration.FileOrganization.PageUtilization));
            var indexPages = Math.Ceiling(primaryPages/Configuration.FileOrganization.IndexRecordsCount);
            var overflowPages = Math.Ceiling(_primaryAreaFile.RecordsCount*Configuration.FileOrganization.OverflowToPrimary);

            indexFile = new IndexFile((int)indexPages);
            primaryAreaFile = new PrimaryAreaFile((int)primaryPages);
            overflowAreaFile = new OverflowAreaFile((int)overflowPages);
        }
        private int GetFirstKey()
        {
            return _primaryAreaFile.GetFirstKey();
        }
        private void Debug()
        {
            //print all information about files
            Console.WriteLine("Index file:");
            Console.WriteLine($"Page count: {_indexFile._pageCount}");
            //Console.WriteLine($"Records count: {_indexFile.RecordsCount}");
            //Console.WriteLine($"Reads: {_indexFile.Reads}");
            // Console.WriteLine($"Writes: {_indexFile.Writes}");
            Console.WriteLine();
            Console.WriteLine("Primary area file:");
            Console.WriteLine($"Page count: {_primaryAreaFile._pageCount}");
            Console.WriteLine($"Records count: {_primaryAreaFile.RecordsCount}");
            //Console.WriteLine($"Reads: {_primaryAreaFile.Reads}");
            //Console.WriteLine($"Writes: {_primaryAreaFile.Writes}");
            Console.WriteLine();
            Console.WriteLine("Overflow area file:");
            Console.WriteLine($"Page count: {_overflowAreaFile._pageCount}");
            Console.WriteLine($"Records count: {_overflowAreaFile.RecordsCount}");
            //Console.WriteLine($"Reads: {_overflowAreaFile.Reads}");
            //Console.WriteLine($"Writes: {_overflowAreaFile.Writes}");
            Console.WriteLine();

        }
    }
}
