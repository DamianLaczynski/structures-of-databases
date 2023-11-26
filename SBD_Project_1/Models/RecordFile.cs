using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SBD_Project_1.Models
{
    internal class RecordFile
    {
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }
        public string Path { get; set; }

        private int _readPointer = 0;

        public RecordFile() 
        {
            ReadCount = 0;
            WriteCount = 0;
        }

        public RecordFile(string path)
        {
            Path = path;
            ReadCount = 0;
            WriteCount = 0;
        }

        public byte[] ReadBlock(int blockSize)
        {
            ReadCount++;
            byte[] block;
            if (File.Exists(Path))
            {
                using (var stream = File.Open(Path, FileMode.Open))
                {
                    stream.Position = _readPointer;
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        if(_readPointer + blockSize > stream.Length)
                        {
                            blockSize = (int)stream.Length - _readPointer;
                        }
                        block = new byte[blockSize];
                        reader.Read(block, 0, blockSize);
                        _readPointer += blockSize;
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File not found");
            }
            return block;
        }

        public void WriteBlock(byte[] block)
        {
            WriteCount++;
            using (var stream = File.Open(Path, FileMode.Append))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(block);
                }
            }
        }

        public void Write(List<Record> records)
        {
            using (var stream = File.Open(Path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (Record r in records)
                    {
                        writer.Write(ArrayConverter.ToByteArray(r.GetContent()));
                    }
                }
            }
        }

        public List<Record> Read()
        {
            byte[] content;
            if (File.Exists(Path))
            {

                using (var stream = File.Open(Path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        content = new byte[stream.Length];
                        reader.Read(content, 0, (int)stream.Length);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File not found");
            }
            return ArrayConverter.ToRecordList(content);
        }

        public long GetLength()
        {             
            if (File.Exists(Path))
            {
                FileInfo info = new FileInfo(Path);
                return info.Length;
            }
            else
            {
                throw new FileNotFoundException("File not found");
            }
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
            string result = $"File path: {Path}\nRead count: {ReadCount}\nWrite count: {WriteCount}";
            int i = 1;
            foreach (var r in Read())
            {
                result += $"\n{i}: {r}";
                i++;
            }
            return result;
        }
    }
}
