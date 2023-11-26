using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class SortingEngine
    {
        private readonly RecordFile _file;
        private readonly int _tapesCount = Configuration.TAPES_COUNT;
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

            //calculate distribution
            int[] distribution = CalculateDistribution(_sourceTape.GetSeriesCount());

            //distribute
            if (distribution is not null)
                Distribute(distribution);

        }

        private void CreateTapes()
        {
            //define sourse tape
            _sourceTape = new Tape(_file);
            //define tapes for distribution
            _tapes = new List<Tape>(_tapesCount);
            for (int i = 0; i < _tapesCount-1; i++)
            {
                _tapes.Add(new Tape(TapeMode.Write));
            }
            _tapes.Add(_sourceTape);
        }

        public RecordFile Sort()
        {
            Init();
            while (_tapes.Select(t => t.GetSeriesCount()).Sum() > 1)
            {
                Step();
            }
            foreach (Tape t in _tapes)
            {
                t.Close();
            }
            var result = _tapes.Find(t => t.GetSeriesCount() == 1);
            return result.GetFile();
        }

        void Step()
        {
            changeTapesMode();

            Record[] records = new Record[Configuration.TAPES_COUNT-1];
            for(int i = 0; i < Configuration.TAPES_COUNT-1; i++)
            {
                records[i] = null;
            }
            int[] seriesLength = new int[Configuration.TAPES_COUNT-1];
            while (!_readingTapes.Any(t => t.GetSeriesCount() == 0))
            {
                for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
                {
                    seriesLength[i] = _readingTapes[i].GetSeriesLength();
                }
                while (seriesLength.Sum() > 0 || records.Any(r => r != null))
                {
                    for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
                    {
                        if (seriesLength[i] > 0)
                        {
                            if (records[i] is null)
                            {
                                records[i] = _readingTapes[i].GetRecord();
                                seriesLength[i]--;
                            }
                        }
                    }
                    //find min and set to writting tape
                    var temp = records.ToList().Min(_comparer);
                    for (int i = 0; i < Configuration.TAPES_COUNT-1; i++)
                    {
                        if (records[i] is not null && records[i].Equals(temp))
                        {
                            _writtingTape.SetRecord(records[i]);
                            records[i] = null;
                        }
                    }
                }
                _writtingTape.EndOfSeries();
            }

        }

        private int[] CalculateDistribution(int seriesCount)
        {
            return FibonacciSequenceGenerator.GenerateDistribution(_tapesCount-1, seriesCount);
        }

        //search tape that is empty and can be writtingTape
        private Tape findTapeToWritting()
        {
            foreach (Tape t in _tapes)
            {
                if (t.IsEmpty() && t.GetMode() == TapeMode.Read)
                {
                    t.SetMode(TapeMode.Write);
                    return t;
                }
            }
            throw new Exception("There is no empty tape for writting");
        }

        //set TapeMode to Read for tapes that is not WrittingTape
        // !!! use only after findTapeToWritting
        private void setTapesForReading()
        {
            if (_writtingTape is null)
            {
                throw new NullReferenceException("Writting Tape is null");
            }
            else
            {
                foreach (Tape t in _tapes)
                {
                    if (t != _writtingTape)
                    {
                        t.SetMode(TapeMode.Read);

                    }
                }
            }
        }

        private void changeTapesMode()
        {
            //preapare tape for writting
            //find tape that is empty and before was in read mode
            _writtingTape = _tapes.Find(t => t.GetMode() == TapeMode.Read && t.IsEmpty());
            _writtingTape.SetMode(TapeMode.Write);
            Console.WriteLine($"Writting tape: {_writtingTape}");

            //preapare tapes for reading
            //find tapes that are not writting tape and set mode to read
            _readingTapes = _tapes.FindAll(t => t != _writtingTape).ToArray();
            _readingTapes.ToList().ForEach(t => t.SetMode(TapeMode.Read));
            Console.WriteLine($"Reading tapes: {string.Join(", ", _readingTapes.ToList())}");
        }

        public void GetSequece()
        {
            throw new NotImplementedException();
        }

        private void Distribute(int[] distribution)
        {
            for (int i = 0; i < _tapesCount-1; i++)
            {
                for (int j = 0; j < distribution[i]; j++)
                {
                    Record record = _sourceTape.GetRecord();
                    Console.WriteLine($"{j}Move to {i} tape:{record}");
                    _tapes[i].SetRecord(record);
                    _tapes[i].EndOfSeries();
                }
            }
        }
    }
}
