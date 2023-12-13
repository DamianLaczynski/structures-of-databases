using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1.PoliphaseSort
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

        /// <summary>
        /// Initializes tapes and get start distribution
        /// </summary>
        private void Init()
        {
            //define sourse tape
            CreateTapes();
            Console.WriteLine("Start distributing");
            _runsCount = FibonacciDistibution.Distribute(_sourceTape, _tapes);
            Console.WriteLine("End distributing");
        }

        /// <summary>
        /// Prepares tapes for sorting
        /// </summary>
        private void CreateTapes()
        {
            //define sourse tape
            _sourceTape = new Tape(_file);
            //define tapes for distribution
            _tapes = new List<Tape>(Configuration.TAPES_COUNT);
            for (int i = 0; i < Configuration.TAPES_COUNT - 1; i++)
            {
                _tapes.Add(new Tape(TapeMode.Write));
            }
            _tapes.Add(_sourceTape);
        }

        /// <summary>
        /// Sorts file by polifase method
        /// </summary>
        /// <returns>Results of sorting</returns>
        /// <exception cref="Exception"></exception>
        public Results Sort()
        {
            Init();
            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Start sorting");
            var runs = _tapes.Select(t => t.SeriesCount + t.EmptySeriesCount).Sum();
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

            var outTape = _tapes.Find(t => t.SeriesCount == 1);
            string destinationFileName = "result.bin";

            try
            {
                File.Copy(outTape.GetFile().GetPath(), destinationFileName, true);
                Console.WriteLine($"Plik {outTape.GetFile().GetPath()} został skopiowany do {destinationFileName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }

            RecordFile outFile = new RecordFile(destinationFileName, FileMode.OpenOrCreate);

            if (outFile is null)
            {
                throw new Exception("Out file not found");
            }

            return new Results(outFile,
                _tapes.Select(t => t.GetReadsCount()).Sum(),
                _tapes.Select(t => t.GetWritesCount()).Sum(),
                stopwatch.ElapsedMilliseconds,
                phaseCounter,
                runs);
        }

        public void Clear()
        {
            _tapes.ToList().ForEach(_tapes => _tapes.Delete());
        }

        /// <summary>
        /// One phase of sorting
        /// </summary>
        void Step()
        {
            ChangeTapesMode();

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
                    SeriesSetter.SetAndMerge(_readingTapes, _writtingTape, _comparer);
                }
            }
        }

        /// <summary>
        /// Prepares tapes for next phase
        /// </summary>
        private void ChangeTapesMode()
        {
            //preapare tape for writting
            //find tape that is empty and before was in read mode
            _writtingTape = _tapes.Find(t => t.GetMode() == TapeMode.Read && t.IsEmpty());
            _writtingTape.SetMode(TapeMode.Write);
            Console.WriteLine($"Writting tape:");
            _writtingTape.Print();

            //preapare tapes for reading
            //find tapes that are not writting tape and set mode to read
            _readingTapes = _tapes.FindAll(t => t != _writtingTape).ToArray();
            _readingTapes.ToList().ForEach(t => t.SetMode(TapeMode.Read));
            Console.WriteLine($"Reading tapes:");
            _readingTapes.ToList().ForEach(t => { t.Print(); });
            Console.WriteLine();
            Console.ReadKey();
        }


    }
}
