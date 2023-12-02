using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SBD_Project_1
{
    public enum TapeMode
    {
        Read = 1,
        Write = 2
    }
    internal class Tape
    {

        private Queue<Record> _queue = new Queue<Record>();

        private static int _tapeNumber = 1;
        private TapeMode _mode;
        private RecordFile _file;

        public long SeriesCount { get; set; }
        public long EmptySeriesCount {  get; set; }

        public Tape(TapeMode mode)
        {
            _tapeNumber++;
            _mode = mode;
            _file = CreateFile();
        }

        public Tape(RecordFile file)
        {
            _mode = TapeMode.Read;
            _file = file;
        }

        public Record GetRecord()
        {
            if (_mode == TapeMode.Write)
            {
                throw new Exception("Mode: READ. You cannot read now.");
            }
            TryRefillBuffer();
            if (_queue.Count == 0)
            {
                return null;
            }
            return _queue.Dequeue();
        }
        public Record GetNextRecord()
        {
            if (_mode == TapeMode.Write)
            {
                throw new Exception("Mode: READ. You cannot read now.");
            }
            TryRefillBuffer();
            if(_queue.Count == 0)
            {
                return null;
            }
            return _queue.Peek();
        }
        public void SetRecord(Record record)
        {
            if(_mode == TapeMode.Read)
            {
                throw new Exception("Mode: READ. You cannot write now.");
            }
            this._queue.Enqueue(record);
            if (_queue.Count == Configuration.MAX_RECORDS_IN_BUFFER)
            {
                _file.WriteBlock(ArrayConverter.ToByteArray(this._queue));
                _queue.Clear();
            }
        }

        public void SetMode(TapeMode mode)
        {
            if(_queue.Count == 0)
            {
                
                if(mode == TapeMode.Read && this._mode != TapeMode.Read)
                {
                    _file.SetReadPointer(0);
                }
                else if(mode == TapeMode.Write)
                {
                    _file.OverrideFile();
                }
                this._mode = mode;
            }
            else
            {
                if(this._mode == TapeMode.Read && mode == TapeMode.Write)
                {
                    throw new Exception("There is any content; Mode: READ");
                }
                else if(this._mode == TapeMode.Write)
                {
                    _file.WriteBlock(ArrayConverter.ToByteArray(this._queue));
                    _queue.Clear();
                    SetMode(mode);
                }
            }
        }
        public void Close()
        {
            if(_mode == TapeMode.Write)
            {
                _file.WriteBlock(ArrayConverter.ToByteArray(this._queue));
                _queue.Clear();
            }
        }

        public TapeMode GetMode() { return _mode; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return _file.ToString() + $"SeriesCount:{this.SeriesCount}\nEmptySeriesCount:{this.EmptySeriesCount}\n";
        }

        public bool IsEmpty()
        {
            TryRefillBuffer();
            if(_queue.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TryRefillBuffer()
        {
            if(_queue.Count == 0)
            {
                byte[] temp = _file.ReadBlock(Configuration.BUFFER_SIZE);
                _queue = ArrayConverter.ToRecordQueue(temp);
            }
        }

        public RecordFile GetFile()
        {
            _file.SetReadPointer(0);
            return _file;
        }

        private RecordFile CreateFile()
        {
            var path = "tape" + _tapeNumber + ".bin";
            return new RecordFile(path, FileMode.Create);
        }
        public string GetName()
        {
            return _file.GetName();
        }

        public long GetReadsCount()
        {
            return _file.ReadCount;
        }
        public long GetWritesCount()
        {
            return _file.WriteCount;
        }
    }
}
