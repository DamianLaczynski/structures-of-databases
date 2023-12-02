﻿using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private long _runsCount = 0;

        public SortingEngine(RecordFile file)
        {
            _file = file;
        }

        private void Init()
        {
            //define sourse tape
            CreateTapes();
            Console.WriteLine("Start distributing");
            _runsCount = FibonacciDistibution.Distribute(_sourceTape, _tapes);
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

        public Results Sort()
        {
            Init();
            Stopwatch stopwatch = new Stopwatch();
            
            Console.WriteLine("Start sorting");
            stopwatch.Start();
            int phaseCounter = 0;
            while (_tapes.Select(t => t.SeriesCount + t.EmptySeriesCount).Sum() > 1)
            {
                Step();
                phaseCounter++;
            }
            stopwatch.Stop();
            Console.WriteLine("End sorting");
            Console.WriteLine($"Phases: {phaseCounter}");
            foreach (Tape t in _tapes)
            {
                t.Close();
            }
            var outFile = _tapes.Find(t => t.SeriesCount == 1);

            if (outFile is null)
            {
                throw new Exception("Out file not found");
            }

            return new Results(outFile.GetFile(), 
                _tapes.Select(t => t.GetReadsCount()).Sum(), 
                _tapes.Select(t => t.GetWritesCount()).Sum(), 
                stopwatch.ElapsedMilliseconds,
                phaseCounter, 
                _runsCount);
        }

        void Step()
        {
            changeTapesMode();

            while (!_readingTapes.Any(t => t.SeriesCount == 0))
            {
                if (_readingTapes.Any(t => t.EmptySeriesCount > 0))
                {
                    SeriesSetter.SetSerie(_readingTapes.ToList().Find(t => t.EmptySeriesCount == 0), _writtingTape);
                    _readingTapes.ToList().Find(t => t.EmptySeriesCount == 0).SeriesCount--;
                    _readingTapes.ToList().Find(t => t.EmptySeriesCount > 0).EmptySeriesCount--;
                }
                else
                {
                    SetAndMerge(_readingTapes, _writtingTape);
                }
            }
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
