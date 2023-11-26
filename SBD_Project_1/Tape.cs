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

        private static readonly int BufferSize = int.Parse(Configuration.configuration["bufferSize"]);
        private readonly int MAX_RECORDS_IN_BUFFER = BufferSize/(15*sizeof(int));
        private Queue<Record> _queue = new Queue<Record>();

        private static int _tapeNumber = 1;
        private TapeMode _mode;
        private RecordFile _file;

        private int _seriesCount = 0;
        private List<int> _seriesLengths = new List<int>();

        public Tape()
        {
            _tapeNumber++;
            _file = new RecordFile();
            _file.Path = "tape" + _tapeNumber + ".bin";
            if (!System.IO.File.Exists(_file.Path))
            {
                System.IO.File.Create(_file.Path);
            }
        }
        public Tape(TapeMode mode)
        {
            _tapeNumber++;
            _file = new RecordFile();
            _file.Path = "tape" + _tapeNumber + ".bin";
            _mode = mode;

            //create file if not exists
            if (!System.IO.File.Exists(_file.Path))
            {
                System.IO.File.Create(_file.Path);
            }
        }

        public Tape(RecordFile file)
        {
            _file = file;
            _mode = TapeMode.Read;
            var recordsCount = (int)(_file.GetLength()/ (sizeof(int) * Configuration.MAX_RECORD_LENGTH));
            for (int i = 0; i < recordsCount; i++)
            {
                _seriesLengths.Add(1);
            }
        }
        public Record[] GetSeries()
        {
            Record[] series = new Record[_seriesLengths.First()];
            for (int i = 0; i < series.Length; i++)
            {
                series[i] = GetRecord();
            }
            return series;
        }
        public void SetSeries(Record[] series)
        {
            foreach (var record in series)
            {
                SetRecord(record);
            }
            _seriesLengths.Add(series.Length);
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
            if (_seriesLengths.First() == 1)
            {
                _seriesLengths.RemoveAt(0);
            }
            else
            {
                //decrement series length
                var length = _seriesLengths.First();
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
            if (_queue.Count == MAX_RECORDS_IN_BUFFER)
            {
                _file.WriteBlock(ArrayConverter.ToByteArray(this._queue));
                _queue.Clear();
            }
        }

        public void SetMode(TapeMode mode)
        {
            if(_queue.Count == 0)
            {
                this._mode = mode;
                //TODO: clear file or set pointer to 0
            }
            else
            {
                if(this._mode == TapeMode.Read)
                {
                    throw new Exception("There is any content; Mode: READ");
                }
                else
                {
                    _file.WriteBlock(ArrayConverter.ToByteArray(this._queue));
                    _queue.Clear();
                }
            }
        }

        public TapeMode GetMode() { return _mode; }
        public bool IsEmpty()
        {
            return _queue.Count == 0;
        }

        public int GetSeriesCount()
        {
            return _seriesLengths.Count;
        }

        public void SetSeriesCount(int seriesCount)
        {
            _seriesCount = seriesCount;
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
    }
}
