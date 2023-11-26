using SBD_Project_1;
using SBD_Project_1.Models;
using SBD_Project_1.RecordGeneration;

RecordFile file = new RecordFile("test.bin");
RecordFilesGenerator generator = new RecordFilesGenerator(file);
generator.Generate(Configuration.RECORDS_COUNT);
Console.WriteLine(file);
SortingEngine engine = new SortingEngine(file);
var result = engine.Sort();
Console.WriteLine("Result");
Console.WriteLine(result);
