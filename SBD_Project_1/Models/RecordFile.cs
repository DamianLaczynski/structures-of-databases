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
        public long Reads { get; set; }
        public long Writes { get; set; }
        private string Path { get; set; }

        private long _readPointer = 0;

        public RecordFile(string path, FileMode mode)
        {
            Path = path;
            Reads = 0;
            Writes = 0;

            SetFileMode(mode);
        }

        /// <summary>
        /// Sets file mode.
        /// </summary>
        /// <param name="mode">FileMode to set</param>
        /// <exception cref="FileNotFoundException">If you want set open mode to not exists file</exception>
        /// <exception cref="ArgumentException">Unsupported file mode</exception>
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

        /// <summary>
        /// Reads block of bytes from file.
        /// </summary>
        /// <param name="blockSize">The size of block in bytes.</param>
        /// <returns>Block of bytes from file.
        /// If in file is no more bytes to read, returns empty array.</returns>
        public byte[] ReadBlock(int blockSize)
        {
            byte[] block;

            using (var stream = File.Open(Path, FileMode.Open))
            {
                //set steam position to read pointer
                stream.Position = _readPointer;
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    //if block size is bigger than current file content, set block size to current file content size
                    if (_readPointer + blockSize > stream.Length)
                    {
                        blockSize = (int)(stream.Length - _readPointer);
                    }
                    block = new byte[blockSize];
                    //read only if block size is bigger than 0
                    if (blockSize > 0)
                    {
                        reader.Read(block, 0, blockSize);
                        _readPointer += blockSize;

                        Reads++;
                    }
                }
            }

            return block;
        }

        /// <summary>
        /// Append current file with block of bytes
        /// </summary>
        /// <param name="block">Block of bytes</param>
        public void WriteBlock(byte[] block)
        {
            if(block.Length == 0)
            {
                return;
            }
            using (var stream = File.Open(Path, FileMode.Append))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(block);

                    Writes++;
                }
            }
        }

        /// <summary>
        /// Writes records to file
        /// </summary>
        /// <param name="records">List of Records</param>
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

        /// <summary>
        /// Reads all records from file.
        /// </summary>
        /// <returns>List of Records</returns>
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

        ///<returns>The size of current file in bytes.</returns>
        public long GetLength()
        {
            FileInfo info = new FileInfo(Path);
            return info.Length;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Value of read pointer to set</param>
        public void SetReadPointer(long position)
        {
            _readPointer = position;
        }

        /// <summary>
        /// Sets read pointer to the beginning of file.
        /// </summary>
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

        /// <summary>
        /// Prints file name and all records in file.
        /// </summary>
        public void Print()
        {
            //print file info
            Console.WriteLine(GetName());

            //print all records in file
            int i = 1;
            foreach (var r in Read())
            {
                Console.WriteLine($"{i}: {r}");
                i++;
            }
        }

        public override string? ToString()
        {
            string result = $"File path: {Path}\nRead count: {Reads}\nWrite count: {Writes}\n";
            return result;
        }

        /// <summary>
        /// Checks if file is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return GetLength() == 0;
        }
        /// <summary>
        /// Returns file name.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Path.Split('\\').Last();
        }

        public void Delete()
        {
            File.Delete(Path);
        }
        public string GetPath()
        {
            return Path;
        }
    }
}
