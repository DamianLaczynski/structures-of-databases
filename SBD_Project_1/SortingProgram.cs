using SBD_Project_1.Models;
using SBD_Project_1.RecordGeneration;
using SBD_Project_1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class SortingProgram
    {
        public void Start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Generate new record file");
                Console.WriteLine("2. User input");
                Console.WriteLine("3. Choose file");
                Console.WriteLine("4. Display file");
                Console.WriteLine("0. Exit");

                Console.Write("Choose option: ");

                string input = Console.ReadLine();

                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            Gen();
                            break;
                        case 2:
                            Input();
                            break;
                        case 3:
                            ChooseFile();
                            break;
                        case 4:
                            DisplayFile();
                            break;
                        case 0:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Wrong choice. Press any key.");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Try again");
                    Console.ReadKey();
                }
            }

        }

        private void Gen()
        {
            Console.WriteLine("Enter records number: ");
            var input = Console.ReadLine();
            RecordFile file = new RecordFile("gen.bin", FileMode.Create);
            RecordFilesGenerator generator = new RecordFilesGenerator(file);
            int.TryParse(input, out int num);
            generator.Generate(num);
            file.Print();
            Console.ReadKey();
        }

        static void Input()
        {
            Console.WriteLine("Enter records number: ");
            var input = Console.ReadLine();
            RecordFile file = new RecordFile("input.bin", FileMode.Create);
            RecordFilesGenerator generator = new RecordFilesGenerator(file);
            int.TryParse(input, out int num);
            generator.EnterRecords(num);
            file.Print();
            Console.ReadKey();
        }

        private void ChooseFile()
        {
            string directoryPath = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(directoryPath, "*.bin");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("List of files:");

                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }

                Console.WriteLine("0. Wyjście");

                Console.Write("Choose file, -1 - back, 0 - exit: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int choice) && choice >= -1 && choice <= files.Length)
                {
                    if (choice == 0)
                    {
                        Environment.Exit(0);
                    }
                    if (choice == -1)
                    {
                        break;
                    }

                    string selectedFilePath = files[choice - 1];
                    Console.WriteLine($"Selected file: {selectedFilePath}");

                    string destinationFileName = "source.bin";

                    try
                    {
                        File.Copy(selectedFilePath, destinationFileName, true);
                        Console.WriteLine($"File {selectedFilePath} copied to {destinationFileName}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    RecordFile file = new RecordFile(destinationFileName, FileMode.OpenOrCreate);
                    Sort(file);

                    Console.WriteLine("Press any key.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Wrong choise. Press any key");
                    Console.ReadKey();
                }
            }
        }
        private void DisplayFile()
        {
            string directoryPath = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(directoryPath, "*.bin");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("List og file:");

                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }

                Console.WriteLine("0. Exit");

                Console.Write("Choose file or enter 0 for exit: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int choice) && choice >= 0 && choice <= files.Length)
                {
                    if (choice == 0)
                    {
                        Environment.Exit(0);
                    }

                    string selectedFilePath = files[choice - 1];
                    Console.WriteLine($"Selected file: {selectedFilePath}");

                    RecordFile file = new RecordFile(selectedFilePath, FileMode.OpenOrCreate);
                    file.Print();

                    Console.WriteLine("Press any key.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Wrong choise. Press any key");
                    Console.ReadKey();
                }
            }
        }

        private void Sort(RecordFile file)
        {
            SortingEngine engine = new SortingEngine(file);
            var result = engine.Sort();
            engine.Clear();
            Console.Clear();
            result.Print();
            Console.ReadKey();
        }

    }
}
