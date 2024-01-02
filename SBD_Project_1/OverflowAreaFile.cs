using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class OverflowAreaFile
    {
        public int MaxRecords { get; set; }
        public int MaxIndex { get; set; }
        public int Writes { get; set; }
        public int Reads { get; set; }
        public int RecordsCount { get; set; }
        public int _pageCount { get; set; }

        private static int OverflowAreaFileNo { get; set; } = 0;

        private BinaryFile _file;

        private LinkedList<Record> _records;

        public OverflowAreaFile() 
        {
            _file = new BinaryFile(@$"fileOrganization\overflow{OverflowAreaFileNo}.bin");
            _records = new LinkedList<Record>();
            _pageCount = _file.GetPageCount();
            OverflowAreaFileNo++;
        }

        public OverflowAreaFile(int pagesCount)
        {
            _file = new BinaryFile(@$"fileOrganization\overflow{OverflowAreaFileNo}.bin", FileMode.Create);
            _records = new LinkedList<Record>();
            //_pageCount = pagesCount; ??? check this
            OverflowAreaFileNo++;
        }
        private void LoadPage(int pageNo)
        {
            var page = _file.ReadPage(pageNo);
            _records = ArrayConverter.ToLinkedListRecords(page);
            Reads++;
        }
        private void SavePage(int pageNo)
        {
            _file.WritePage(pageNo, ArrayConverter.ToByteArray(_records));
            Writes++;
        }
        public void ResetOperationCount()
        {
            Reads = 0;
            Writes = 0;
        }

        public void AddRecord(Record record)
        {
            LoadPage(_pageCount);

            //add new page if there is no space
            if(_records.Count == Configuration.FileOrganization.OverflowAreaRecordsCount)
            {
                _pageCount++;
                _records = new LinkedList<Record>();
            }

            _records.AddLast(record);
            RecordsCount++;
            SavePage(_pageCount);
        }
        public void AddRecord(int pointer, Record record)
        {
            bool isPointerGreater = false;
            if(pointer > record.Key)
            {
                isPointerGreater = true;
                record.OverflowPointer = pointer;
            }

            this.AddRecord(record);

            var pageNo = 0;
            while(true)
            {
                LoadPage(pageNo);
                var recordToUpdate = _records.FirstOrDefault(x => x.Key == pointer);
                if(recordToUpdate is not null)
                {
                    if(isPointerGreater)
                    {
                        recordToUpdate.OverflowPointer = -1;
                    }
                    else
                    {
                        recordToUpdate.OverflowPointer = record.Key;
                    }
                    SavePage(pageNo);
                    return;
                }
                pageNo++;
            }
        }
        public Record GetRecord(int key)
        {
            Record record;
            for(int i = 0; i <= _pageCount; i++)
            {
                LoadPage(i);
                record = _records.FirstOrDefault(x => x.Key == key);
                if(record is not null)
                {
                    return record;
                }
            }
            throw new Exception("Key not found");
        }
        public Record GetNextRecord(int key)
        {
            Record record;
            for (int i = 0; i <= _pageCount; i++)
            {
                LoadPage(i);
                record = _records.FirstOrDefault(x => x.Key == key);
                if (record is not null)
                {
                    _records.Remove(record);
                    RecordsCount--;
                    return record;
                }
            }
            throw new Exception("Key not found");
        }
        public void RemoveRecord(int key)
        {
            Record record;
            for(int i = 0; i <= _pageCount; i++)
            {
                LoadPage(i);
                record = _records.FirstOrDefault(x => x.Key == key);
                if(record is not null)
                {
                    _records.Remove(record);
                    SavePage(i);
                    RecordsCount--;
                    return;
                }
            }
            throw new Exception("Key not found");
        }
        public void Display()
        {
            var currentPageNo = 0;
            Console.WriteLine("Overflow File");
            while (currentPageNo <= _pageCount)
            {
                LoadPage(currentPageNo);
                foreach (var record in _records)
                {
                    Console.WriteLine(record.ToString());
                }
                currentPageNo++;
            }
        }
    }
}
