using SBD_Project_1;
using SBD_Project_1.Models;
using SBD_Project_1.RecordGeneration;

RecordFile file = new RecordFile("test.bin");
//RecordFilesGenerator generator = new RecordFilesGenerator(file);
//generator.Generate(13);
Console.WriteLine(file);
SortingEngine engine = new SortingEngine(file);
engine.Sort();
