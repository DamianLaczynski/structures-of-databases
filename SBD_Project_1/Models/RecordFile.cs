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
        private string Path { get; set; }

        private int _readPointer = 0;

        private long _recordsCount = 0;

        public RecordFile(string path, FileMode mode)
        {
            Path = path;
            ReadCount = 0;
            WriteCount = 0;

            SetFileMode(mode);
        }

        private void SetFileMode(FileMode mode)
        {
            
            switch (mode)
            {
                case FileMode.Create:
                    if (File.Exists(Path))
                        File.Delete(Path);

                    using (File.Create(Path)) ;
                    break;
                case FileMode.Open:
                    if (!File.Exists(Path))
                        throw new FileNotFoundException("File not found");
                    break;
                case FileMode.OpenOrCreate:
                    if (!File.Exists(Path))
                    {
                        using (File.Create(Path)) ;
                    }
                    break;
                default:
                    throw new ArgumentException("Wrong file mode");
            }
        }

        public byte[] ReadBlock(int blockSize)
        {
            ReadCount++;

            byte[] block;
            using (var stream = File.Open(Path, FileMode.Open))
            {
                stream.Position = _readPointer;
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    if (_readPointer + blockSize > stream.Length)
                    {
                        blockSize = (int)stream.Length - _readPointer;
                    }
                    block = new byte[blockSize];
                    reader.Read(block, 0, blockSize);
                    _readPointer += blockSize;
                }
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

            using (var stream = File.Open(Path, FileMode.Open))
            {
                stream.Position = _readPointer;
                long blockSize = stream.Length - _readPointer;
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    if (stream.Length == 0)
                    {
                        return new List<Record>();
                    }
                    else
                    {
                        content = new byte[blockSize];
                        reader.Read(content, 0, (int)blockSize);
                    }
                }
            }

            return ArrayConverter.ToRecordList(content);
        }

        public long GetLength()
        {
            FileInfo info = new FileInfo(Path);
            return info.Length;
        }

        public void SetReadPointer(long position)
        {
            _readPointer = (int)position;
        }

        public void OverrideFile()
        {
            using (var stream = File.Open(Path, FileMode.Truncate))
            {
                stream.SetLength(0);
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
        public void Print()
        {
            //print file info
            Console.WriteLine(this.ToString());

            //print all records in file
            int i = 1;
            foreach (var r in Read())
            {
                Console.WriteLine($"{i}: {r}\n");
                i++;
            }
        }

        public override string? ToString()
        {
            string result = $"File path: {Path}\nRead count: {ReadCount}\nWrite count: {WriteCount}\n";
            return result;
        }
    }
}
