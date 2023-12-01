using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class SortingEngine
    {
        private readonly RecordFile _file;
        private List<Tape> _tapes;
        private Tape _sourceTape;

        private Tape _writtingTape;
        private Tape[] _readingTapes;
        private MorePrimeNumbersFirstComparer _comparer = new MorePrimeNumbersFirstComparer();

        public SortingEngine(RecordFile file)
        {
            _file = file;
        }

        private void Init()
        {
            //define sourse tape
            CreateTapes();
            Console.WriteLine("Start distributing");
            Distribute();
            Console.WriteLine("End distributing");
        }

        private void CreateTapes()
        {
            //define sourse tape
            _sourceTape = new Tape(_file);
            //define tapes for distribution
            _tapes = new List<Tape>(Configuration.TAPES_COUNT);
            for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
            {
                _tapes.Add(new Tape(TapeMode.Write));
            }
            _tapes.Add(_sourceTape);
        }

        public RecordFile Sort()
        {
            Init();
            Console.WriteLine("Start sorting");
            while (_tapes.Select(t => t.SeriesCount + t.EmptySeriesCount).Sum() > 1)
            {
                Step();
            }
            Console.WriteLine("End sorting");
            foreach (Tape t in _tapes)
            {
                t.Close();
            }
            var result = _tapes.Find(t => t.SeriesCount == 1);
            return result.GetFile();
        }

        void Step()
        {
            changeTapesMode();

            while (!_readingTapes.Any(t => t.SeriesCount == 0))
            {
                if (_readingTapes.Any(t => t.EmptySeriesCount > 0))
                {
                    SetSerie(_readingTapes.ToList().Find(t => t.EmptySeriesCount == 0), _writtingTape);
                    _readingTapes.ToList().Find(t => t.EmptySeriesCount == 0).SeriesCount--;
                    _readingTapes.ToList().Find(t => t.EmptySeriesCount > 0).EmptySeriesCount--;
                }
                else
                {
                    SetAndMerge(_readingTapes, _writtingTape);
                }
            }
        }

        private int[] CalculateDistribution(int seriesCount)
        {
            return FibonacciSequenceGenerator.GenerateDistribution(Configuration.TAPES_COUNT-1, seriesCount);
        }

        private void changeTapesMode()
        {
            //preapare tape for writting
            //find tape that is empty and before was in read mode
            _writtingTape = _tapes.Find(t => t.GetMode() == TapeMode.Read && t.IsEmpty());
            _writtingTape.SetMode(TapeMode.Write);
            //Console.WriteLine($"Writting tape: {_writtingTape}");

            //preapare tapes for reading
            //find tapes that are not writting tape and set mode to read
            _readingTapes = _tapes.FindAll(t => t != _writtingTape).ToArray();
            _readingTapes.ToList().ForEach(t => t.SetMode(TapeMode.Read));
            //Console.WriteLine($"Reading tapes: {string.Join(", ", _readingTapes.ToList())}");
        }

        //in _tapes last tape is source tape


        private void Distribute()
        {
            Record[] lastOnTape = new Record[Configuration.TAPES_COUNT-1];
            for(int j = 0; j < Configuration.TAPES_COUNT-1; j++)
            {
                lastOnTape[j] = null;
            }
            int i = 0;
            while (!_sourceTape.IsEmpty())
            {
                for (int j = 0; j < FibonacciSequenceGenerator.Get(i); j++)
                {
                    if (lastOnTape[i % (Configuration.TAPES_COUNT - 1)] is not null &&
                        _sourceTape.GetNextRecord() is not null &&
                        lastOnTape[i % (Configuration.TAPES_COUNT - 1)].Index < _sourceTape.GetNextRecord().Index)
                    {
                        lastOnTape[i%(Configuration.TAPES_COUNT-1)] = SetSerie(_sourceTape, _tapes[i%(Configuration.TAPES_COUNT-1)]);
                        _tapes[i%(Configuration.TAPES_COUNT-1)].SeriesCount--;
                    }
                    lastOnTape[i%(Configuration.TAPES_COUNT-1)] = SetSerie(_sourceTape, _tapes[i%(Configuration.TAPES_COUNT-1)]);
                }
                i++;
            }
        }

        private Record SetSerie(Tape source, Tape destination)
        {
            Record last = null;
            while (true)
            {
                Record record = source.GetRecord();
                if (record is null)
                {
                    destination.EmptySeriesCount++;
                    
                    break;
                }
                last = record;
                destination.SetRecord(record);
                //Console.WriteLine($"Move to {destination.GetName()}:{record}");
                Record nextRecord = source.GetNextRecord();
                if (nextRecord is null)
                {
                    destination.SeriesCount++;
                    
                    break;
                }
                else if (record.Index > nextRecord.Index)
                {
                    destination.SeriesCount++;
                    
                    break;
                }
            }
            return last;
        }

        private void SetAndMerge(Tape[] sources, Tape destination)
        {
            Record[] records = new Record[Configuration.TAPES_COUNT-1];
            bool[] isEndOfSeries = new bool[Configuration.TAPES_COUNT-1];
            for(int i = 0; i < Configuration.TAPES_COUNT-1; i++)
            {
                records[i] = null;
                isEndOfSeries[i] = false;
            }
            while (!isEndOfSeries.All(b => b == true))
            {
                for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
                {
                    if (records[i] is null && !isEndOfSeries[i])
                    {
                        records[i] = sources[i].GetRecord();
                        if (records[i] is null)
                        {
                            isEndOfSeries[i] = true;
                            sources[i].SeriesCount--;
                        }
                    }   
                }
                //find min and set to writting tape
                var temp = records.ToList().Min(_comparer);
                for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
                {
                    if (records[i] is not null && records[i].Equals(temp))
                    {
                        //Console.WriteLine($"Move to {destination.GetName()}:{records[i]}");
                        destination.SetRecord(records[i]);
                        if (sources[i].GetNextRecord() is null || sources[i].GetNextRecord().Index < records[i].Index)
                        {
                            isEndOfSeries[i] = true;
                            sources[i].SeriesCount--;
                        }
                        records[i] = null;
                    }
                }
            }
            destination.SeriesCount++;
        }
    }
}
