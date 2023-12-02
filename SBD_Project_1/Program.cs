using SBD_Project_1;
using SBD_Project_1.Models;
using SBD_Project_1.RecordGeneration;

RecordFile file = new RecordFile("test.bin", FileMode.OpenOrCreate);
RecordFilesGenerator generator = new RecordFilesGenerator(file);
generator.Generate(Configuration.RECORDS_COUNT);
file.Print();
SortingEngine engine = new SortingEngine(file);
var result = engine.Sort();
Console.WriteLine("Result");
result.Print();
