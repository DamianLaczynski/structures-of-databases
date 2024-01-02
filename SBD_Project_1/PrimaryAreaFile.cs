using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBD_Project_1
{
    internal class PrimaryAreaFile
    {

        private readonly int _primaryAreaRecordsCount = Configuration.FileOrganization.PrimaryAreaRecordsCount;
        public int MaxRecords { get; set; }
        public int MaxIndex { get; set; }
        public int Writes { get; set; }
        public int Reads { get; set; }

        private static int PrimaryAreaNo { get; set; } = 0;

        public int RecordsCount { get; set; }
        public int _pageCount { get; set; }

        private BinaryFile _file;

        private LinkedList<Record> _records;

        private int _currentPageNo = 0;

        public int RecordsOnCurrentPage { get; set; }

        public PrimaryAreaFile()
        {
            _file = new BinaryFile(@$"fileOrganization\primary{PrimaryAreaNo}.bin");
            _records = new LinkedList<Record>();
            _pageCount = _file.GetPageCount();
            if(_pageCount == 0)
            {
                _pageCount++;
            }
            PrimaryAreaNo++;
        }

        public PrimaryAreaFile(int pagesCount)
        {
            _file = new BinaryFile(@$"fileOrganization\primary{PrimaryAreaNo}.bin", FileMode.Create);
            _records = new LinkedList<Record>();
            _pageCount = pagesCount;
            PrimaryAreaNo++;
        }

        private void Allocate()
        {

        }

        public void LoadPage(int pageNo)
        {
            var page = _file.ReadPage(pageNo);
            _records = ArrayConverter.ToLinkedListRecords(page);
            Reads++;
        }
        public void SavePage(int pageNo)
        {
            _file.WritePage(pageNo, ArrayConverter.ToByteArray(_records));
            Writes++;
        }
        public void ResetOperationCount()
        {
            Reads = 0;
            Writes = 0;
        }
        public int AddRecord(int pageNo, Record record)
        {
            if(pageNo < 0)
            {
                throw new Exception("Page number cannot be negative. Invalid Page number");
            }
            LoadPage(pageNo);

            //key already exists on page
            if (_records.Any(x => x.Key == record.Key))
            {
                throw new Exception("Key already exists");
            }

            //add record to page if there is space
            if (_records.Count < _primaryAreaRecordsCount)
            {
                //add record to the end of the list if it is the biggest key
                if ((_records.Last is not null && _records.Last.Value.Key < record.Key) || _records.Count == 0)
                {
                    _records.AddLast(record);
                }
                //add record before the first bigger key
                else
                {
                    var node = _records.First;
                    while (node != null)
                    {
                        if (node.Value.Key > record.Key)
                        {
                            _records.AddBefore(node, record);
                            break;
                        }
                        node = node.Next;
                    }
                }
                
                SavePage(pageNo);
                RecordsCount++;
                return 0;
            }
            //no space on page
            else
            {
                var beforeNewRecord = _records.Where(x => x.Key < record.Key).Last();
                if (beforeNewRecord.OverflowPointer != -1)
                {
                    if(beforeNewRecord.OverflowPointer > record.Key)
                    {
                        var tmp = beforeNewRecord.OverflowPointer;
                        beforeNewRecord.OverflowPointer = record.Key;
                        SavePage(pageNo);
                        return tmp;
                    }
                    return beforeNewRecord.OverflowPointer;
                }
                beforeNewRecord.OverflowPointer = record.Key;
                SavePage(pageNo);
                return -1;
            }
        }

        public void AddRecord(Record record, ref IndexFile indexFile)
        {
            //if the current page of the new file already contains α · bf records
            if (RecordsOnCurrentPage >= (Configuration.FileOrganization.PageUtilization * Configuration.FileOrganization.PrimaryAreaRecordsCount))
            {
                _currentPageNo++;
                RecordsOnCurrentPage = 0;
                //Insert the key of the new record and the addresss of the new page to the new index.
                indexFile.AddRecord(record.Key, _currentPageNo);
            }
            else if(_records.Count == 0)
            {
                indexFile.AddRecord(record.Key, _currentPageNo);
            }

            this.AddRecord(_currentPageNo, record);
            RecordsOnCurrentPage++;
        }
        public void RemoveRecord(int pageNo, int key)
        {
            LoadPage(pageNo);

            var record = _records.Where(x => x.Key == key).FirstOrDefault();
            if (record is null)
            {
                throw new Exception("Record not found");
            }
            else
            {
                record.MarkAsDeleted();
                RecordsCount--;
            }
            SavePage(pageNo);
        }
        public void UpdateRecord(int pageNo, int key, Record newRecord)
        {
            LoadPage(pageNo);


            var record = _records.Where(x => x.Key == key).FirstOrDefault();
            if (record is null)
            {
                throw new Exception("Record not found");
            }
            else
            {
                record.Update(newRecord);
            }

            SavePage(pageNo);
        }
        public Record GetRecord(int pageNo, int key)
        {
            LoadPage(pageNo);


            var record = _records.Where(x => x.Key == key).FirstOrDefault();
            if (record is null)
            {
                throw new Exception("Record not found");
            }
            else
            {
                return record;
            }
        }
        public void InitReorganization()
        {
            _currentPageNo = 0;
            _records = new LinkedList<Record>();
            _pageCount = _file.GetPageCount();
            LoadPage(_currentPageNo);
        }
        public Record GetNextRecord()
        {

            if (_records.Count == 0 && _currentPageNo < _pageCount)
            {
                _currentPageNo++;
                LoadPage(_currentPageNo);
            }

            var record = _records.First();
            _records.RemoveFirst();
            RecordsCount--;
            return record;
        }

        public void Display()
        {
            var currentPageNo = 0;
            Console.WriteLine("Primary File");
            while (currentPageNo <= _pageCount - 1)
            {
                LoadPage(currentPageNo);
                Console.WriteLine($"Page {currentPageNo}");
                foreach (var record in _records)
                {
                    Console.WriteLine(record.ToString());
                }
                if(_records.Count < _primaryAreaRecordsCount)
                {
                    for(int i = 0; i < _primaryAreaRecordsCount - _records.Count; i++)
                    {
                        Console.WriteLine("Empty");
                    }
                }
                currentPageNo++;
            }
        }
    }
}
