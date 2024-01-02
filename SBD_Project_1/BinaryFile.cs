using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class BinaryFile
    {
        private readonly int _blockSize = Configuration.FileOrganization.PageSize;
        private string _path { get; set; }

        public BinaryFile(string path)
        {
            _path = path;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (File.Create(path)) ;

        }
        public BinaryFile(string path, FileMode mode)
        {
            _path = path;
            switch(mode)
            {
                case FileMode.Open:
                    if (!File.Exists(path))
                    {
                        using (File.Create(path)) ;
                    }
                    break;
                case FileMode.Create:
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (File.Create(path)) ;
                    break;
                default:
                    throw new ArgumentException("This mode is not supported");
                    
            }
        }
        public byte[] ReadPage(int pageNumber)
        {
            //init table with zeros
            byte[] block = new byte[_blockSize];
            for (int i = 0; i < _blockSize; i++)
            {
                block[i] = 0;
            }

            using (var stream = File.Open(_path, FileMode.Open))
            {
                //set stream position to begin of page
                stream.Position = pageNumber * _blockSize;

                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    reader.Read(block, 0, _blockSize);
                }
            }

            return block;
        }
        //WARNING: to verify
        public void WritePage(int pageNumber, byte[] block)
        {
            if (block.Length != _blockSize)
            {
                var newBlock = new byte[_blockSize];
                for (int i = 0; i < block.Length; i++)
                {
                    newBlock[i] = block[i];
                }
                for (int i = block.Length; i < _blockSize; i++)
                {
                    newBlock[i] = 0;
                }
            }
            using (var stream = File.Open(_path, FileMode.Open))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    //set stream position to begin of page
                    stream.Position = pageNumber * _blockSize;
                    writer.Write(block);
                }
            }
        }

        public void AllocateDiskSpace(int pageCount)
        {
            using (var stream = File.Open(_path, FileMode.Create))
            {
                //fill file with zeros
                for (long i = 0; i < _blockSize*pageCount; i++)
                {
                    stream.WriteByte(0);
                }
            }
        }

        public int GetPageCount()
        {
            using (var stream = File.Open(_path, FileMode.Open))
            {
                return (int)Math.Ceiling((double)stream.Length / _blockSize);
            }
        }

        public byte[] Read()
        {
            byte[] content;

            using (var stream = File.Open(_path, FileMode.Open))
            {
                long blockSize = stream.Length;
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    content = new byte[blockSize];
                    reader.Read(content, 0, (int)blockSize);
                }
            }

            return content;
        }

        ~BinaryFile()
        {
            File.Delete(_path);
        }
    }
}
