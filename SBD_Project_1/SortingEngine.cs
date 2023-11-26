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
        private Tape[] _tapes;
        private Tape _sourceTape;

        private Tape _writtingTape;

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
            if(distribution is not null)
                Distribute(distribution);
            
        }

        private void CreateTapes()
        {
            //define sourse tape
            _sourceTape = new Tape(_file);
            //define tapes for distribution
            _tapes = new Tape[_tapesCount];
            for (int i = 0; i < _tapesCount-1; i++)
            {
                _tapes[i] = new Tape(TapeMode.Write);
            }
            _tapes[_tapesCount - 1] = _sourceTape;
        }

        public void Sort()
        {
            Init();
            while(_tapes.Select(t => t.GetSeriesCount()).Sum() > 1)
            {
                Step();
            }
        }

        void Step()
        {
            //preapare tape for writting
            _writtingTape = findTapeToWritting();
            //preapare tapes for reading
            setTapesForReading();
        }

        private int[] CalculateDistribution(int seriesCount)
        {
            return FibonacciSequenceGenerator.GenerateDistribution(_tapesCount-1, seriesCount);
        }

        //search tape that is empty and can be writtingTape
        private Tape findTapeToWritting()
        {
            foreach(Tape t in _tapes)
            {
                if(t.IsEmpty() && t.GetMode() == TapeMode.Read)
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
            if(_writtingTape is null)
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
