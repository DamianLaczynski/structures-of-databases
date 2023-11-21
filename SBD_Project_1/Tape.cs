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

namespace SBD_Project_1
{
    public enum TapeMode
    {
        Read = 1,
        Write = 2
    }
    internal class Tape : RecordFile
    {

        private static readonly int BufferSize = int.Parse(Configuration.configuration["bufferSize"]);
        private static readonly int MaxRecordInBuffer = BufferSize/15*sizeof(int);
        private static int _tapeNumber = 1;
        private Queue<Record> _queue = new Queue<Record>();
        private TapeMode _mode;

        public Tape() : base()
        {
            _tapeNumber++;
            this.Path = "tape" + _tapeNumber + ".dat";
        }
        public Tape(TapeMode mode) : base()
        {
            _tapeNumber++;
            this.Path = "tape" + _tapeNumber + ".dat";
            _mode = mode;
        }

        public Record GetRecord()
        {
            if (_mode == TapeMode.Write)
            {
                throw new Exception("Mode: READ. You cannot read now.");
            }
            if (_queue.Count == 0)
            {
                byte[] temp = this.ReadBlock(BufferSize);
                _queue = ArrayConverter.ToRecordQueue(temp);
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
            if (_queue.Count == MaxRecordInBuffer)
            {
                this.WriteBlock(ArrayConverter.ToByteArray(this._queue));
            }
        }

        public void SetMode(TapeMode mode)
        {
            if(_queue.Count == 0)
            {
                this._mode = mode;
            }
            else
            {
                if(this._mode == TapeMode.Read)
                {
                    throw new Exception("There is any content; Mode: READ");
                }
                else
                {
                    throw new Exception("There is unsaved content; Mode: WRITE");
                }
            }
        }

        public bool IsEmpty()
        {
            return _queue.Count == 0;
        }

    }
}
