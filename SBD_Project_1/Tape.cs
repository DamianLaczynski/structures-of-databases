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

        private int _recordsInSerieCounter = 0;
        private List<int> _seriesLengths = new List<int>();

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
            //TODO: releace with merging records on distribution
            var recordsCount = (int)(_file.GetLength()/ (sizeof(int) * Configuration.MAX_RECORD_LENGTH));
            for (int i = 0; i < recordsCount; i++)
            {
                _seriesLengths.Add(1);
            }

        }

        public void EndOfSeries()
        {
            _seriesLengths.Add(_recordsInSerieCounter);
            _recordsInSerieCounter = 0;
        }

        public Record GetRecord()
        {
            if (_mode == TapeMode.Write)
            {
                throw new Exception("Mode: READ. You cannot read now.");
            }
            if (_queue.Count == 0)
            {
                byte[] temp = _file.ReadBlock(Configuration.BUFFER_SIZE);
                _queue = ArrayConverter.ToRecordQueue(temp);
            }
            //update series count
            if (_seriesLengths.First() == 0)
            {
                _seriesLengths.RemoveAt(0);
                return null;
            }
            else if (_seriesLengths.First() == 1)
            {
                _seriesLengths.RemoveAt(0);
            }
            else
            {
                //decrement series length
                var length = _seriesLengths.First();
                _seriesLengths.RemoveAt(0);
                _seriesLengths.Insert(0, length - 1);
            }
            return _queue.Dequeue();
        }
        public void SetRecord(Record record)
        {
            if(_mode == TapeMode.Read)
            {
                throw new Exception("Mode: READ. You cannot write now.");
            }
            this._queue.Enqueue(record);
            _recordsInSerieCounter++;
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
        public bool IsEmpty()
        {
            return _seriesLengths.Count == 0;
        }

        //returns number of series in tape
        public int GetSeriesCount()
        {
            return _seriesLengths.Count;
        }

        //returns length of first series
        public int GetSeriesLength()
        {
            return _seriesLengths.First();
        }

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
            return _file.ToString();
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
    }
}
