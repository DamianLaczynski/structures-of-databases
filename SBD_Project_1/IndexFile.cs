using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    public class IndexRecord
    {
        public int Key { get; set; }
        public int PageNo { get; set; }

    }
    public class IndexFile
    {
        private readonly int MaxRecordsCount = Configuration.FileOrganization.IndexRecordsCount;
        public static int IndexFileNo { get; set; } = 0;
        public int MaxIndex { get; set; }

        public int Writes { get; set; }
        public int Reads { get; set; }

        public int _pageCount { get; set; }

        private BinaryFile _file;

        private LinkedList<IndexRecord> _records;

        public int _currentPageNo = 0;
        private int _currentRecordsCount = 0;

        public IndexFile()
        {
            _file = new BinaryFile(@$"fileOrganization\index{IndexFileNo}.bin");
            _records = new LinkedList<IndexRecord>();
            _pageCount = _file.GetPageCount();
            IndexFileNo++;
        }

        public IndexFile(int pagesCount)
        {
            _file = new BinaryFile(@$"fileOrganization\index{IndexFileNo}.bin", FileMode.Create);
            _records = new LinkedList<IndexRecord>();
            _pageCount = pagesCount;
            Allocate();
            IndexFileNo++;
        }

        private void Allocate()
        {
            //AddRecord(0);
        }

        public void AddRecord(int key, int pageNo)
        {
            if(_records.Count == MaxRecordsCount)
            {
                _currentPageNo++;
            }
            _records.AddLast(new IndexRecord { Key = key, PageNo = pageNo });
            SavePage(_currentPageNo);
        }
        public void RemoveRecord(int key)
        {
            throw new NotImplementedException();
        }
        public void UpdateRecord(int key, int pageNo)
        {
            throw new NotImplementedException();
        }
        private void LoadPage(int pageNo)
        {
            var page = _file.ReadPage(pageNo);
            _records = ArrayConverter.ToLinkedListIndexRecords(page);
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

        public int GetPageNo(int key)
        {
            if(_pageCount == 0)
            {
                _records.AddFirst(new IndexRecord { Key = key, PageNo = 0 });
                _pageCount++;
                SavePage(0);
                return 0;
            }
            else
            {
                var currentPage = 0;
                int lastOnPage;
                while(true)
                {
                    LoadPage(currentPage);
                    //search for first page where key is greater than given key
                    var pageNo = _records.Where(x => x.Key > key).FirstOrDefault();
                    if(pageNo is null)
                    {
                        //if key is greater than all keys in current page
                        //save last page number on page
                        lastOnPage = _records.Last().PageNo;
                        //if current page is last page
                        //given key can be on the last page
                        if(currentPage < _pageCount - 1)
                        {
                            currentPage++;
                        }
                        else
                        {
                            return _records.Last().PageNo;
                        }
                    }
                    else
                    {
                        return pageNo.PageNo - 1;
                    }
                }
            }
        }

        public void Display()
        {
            var content = _file.Read();
            var records = ArrayConverter.ToLinkedListIndexRecords(content);
            Console.WriteLine("Index File");
            foreach(var record in records)
            {
                Console.WriteLine($"Key: {record.Key} PageNo: {record.PageNo}");
            }
        }
        
    }
}
