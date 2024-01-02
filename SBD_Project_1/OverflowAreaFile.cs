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
        private readonly int MaxRecordsOnPage = Configuration.FileOrganization.OverflowAreaRecordsCount;
        private static int OverflowAreaFileNo { get; set; } = 0;
        //file for records
        private BinaryFile _file;

        private int _currentPageNo = 0;

        //buffer for records
        private LinkedList<Record> _records;

        public int Writes { get; set; }
        public int Reads { get; set; }
        public int RecordsCount { get; set; }
        public int _pageCount { get; set; }

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
            if (_records.Count == Configuration.FileOrganization.OverflowAreaRecordsCount)
            {
                _pageCount++;
                _records = new LinkedList<Record>();
            }

            _records.AddLast(record);
            RecordsCount++;
            SavePage(_pageCount);
        }

        private Record GetNext(int key)
        {
            return GetRecord(key);
        }
        public void AddRecord(int pointer, Record record)
        {
            if(pointer > record.Key)
            {
                record.OverflowPointer = pointer;
                AddRecord(record);
                return;
            }
            else if(pointer < record.Key)
            {
                //search for greatest record that is smaller than record to add
                var current = GetRecord(pointer);
                if(current.OverflowPointer > record.Key) 
                {
                    record.OverflowPointer = current.OverflowPointer;
                    current.OverflowPointer = record.Key;
                    SavePage(_currentPageNo);
                    AddRecord(record);
                    return;
                }
                var next = GetNext(current.OverflowPointer);
                while(next.OverflowPointer < record.Key && next.OverflowPointer != -1)
                {
                    current = next;
                    next = GetNext(current.OverflowPointer);
                }
                if(next.OverflowPointer == -1)
                {
                    next.OverflowPointer = record.Key;
                    SavePage(_currentPageNo);
                    AddRecord(record);
                    return;
                }
                else
                {
                    record.OverflowPointer =next.OverflowPointer;
                    next.OverflowPointer = record.Key;
                    //record.OverflowPointer = next.Key;
                    SavePage(_currentPageNo);
                    AddRecord(record);
                }
                
                
            }



            /*var currentPage = 0;
            int greaterOnPage;
            while (true)
            {
                LoadPage(currentPage);
                
                var greaterRecord = _records.OrderBy(x => x.Key).Where(x => x.Key > record.Key).FirstOrDefault();
                
                if (greaterRecord is null)
                {
                    
                    greaterRecord = _records.OrderBy(x => x.Key).LastOrDefault();
                    if (greaterRecord is null)
                    {
                        AddRecord(record);
                        return;
                    }
                    else
                    {
                        greaterOnPage = greaterRecord.Key;
                        if(currentPage == _pageCount)
                        {
                            record.OverflowPointer = greaterRecord.Key;
                            AddRecord(record);
                            return;
                        }
                    }
                }
                else
                {
                    record.OverflowPointer = greaterRecord.OverflowPointer;
                    greaterRecord.OverflowPointer = record.Key;
                    SavePage(currentPage);
                }
                currentPage++;

            }*/





            /*
                        bool isPointerGreater = false;
                        if (pointer > record.Key)
                        {
                            isPointerGreater = true;
                            record.OverflowPointer = pointer;
                        }

                        this.AddRecord(record);

                        var pageNo = -1;
                        bool isDirtyPage = false;
                        Record recordToUpdate = null;
                        bool isRecordFound = false;
                        while (!isRecordFound)
                        {
                            pageNo++;
                            LoadPage(pageNo);
                            recordToUpdate = _records.FirstOrDefault(x => x.Key == pointer);
                            if (recordToUpdate is null)
                            {
                                continue;
                            }
                            while (recordToUpdate.OverflowPointer != -1)
                            {
                                recordToUpdate = _records.FirstOrDefault(x => x.Key == recordToUpdate.OverflowPointer);

                                if (recordToUpdate is null)
                                {
                                    break;
                                }
                                pointer = recordToUpdate.OverflowPointer;
                                if (pointer > record.Key)
                                {
                                    isPointerGreater = true;
                                    record.OverflowPointer = pointer;
                                }
                            }
                        }


                        if (isPointerGreater)
                        {
                            recordToUpdate.OverflowPointer = -1;
                        }
                        else
                        {
                            recordToUpdate.OverflowPointer = record.Key;
                            isDirtyPage = true;

                        }
                        if (isDirtyPage)
                        {
                            SavePage(pageNo);
                            isDirtyPage = false;
                        }*/
        }
        public Record GetRecord(int key)
        {
            Record record;
            for (int i = 0; i <= _pageCount; i++)
            {
                _currentPageNo = i;
                LoadPage(_currentPageNo);
                record = _records.FirstOrDefault(x => x.Key == key);
                if (record is not null)
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
                _currentPageNo = i;
                LoadPage(_currentPageNo);
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
            for (int i = 0; i <= _pageCount; i++)
            {
                LoadPage(i);
                record = _records.FirstOrDefault(x => x.Key == key);
                if (record is not null)
                {
                    record.MarkAsDeleted();
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
