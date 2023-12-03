using SBD_Project_1;
using SBD_Project_1.Models;
using SBD_Project_1.RecordGeneration;



while (true)
{
    Console.Clear();
    Console.WriteLine("Menu:");
    Console.WriteLine("1. Generate new record file");
    Console.WriteLine("2. User input");
    Console.WriteLine("3. Choose file");
    Console.WriteLine("4. Display file");
    Console.WriteLine("0. Wyjście");

    Console.Write("Wybierz opcję: ");

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
                Console.WriteLine("Nieprawidłowy wybór. Wciśnij dowolny klawisz, aby spróbować ponownie.");
                Console.ReadKey();
                break;
        }
    }
    else
    {
        Console.WriteLine("Nieprawidłowy format. Wciśnij dowolny klawisz, aby spróbować ponownie.");
        Console.ReadKey();
    }


    static void Gen()
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

    static void ChooseFile()
    {
        string directoryPath = Directory.GetCurrentDirectory();
        string[] files = Directory.GetFiles(directoryPath, "*.bin");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Lista plików:");

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
            }

            Console.WriteLine("0. Wyjście");

            Console.Write("Wybierz numer pliku lub -1 aby wrócić do menu lub 0 aby wyjść: ");
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
                Console.WriteLine($"Wybrano plik: {selectedFilePath}");

                string destinationFileName = "source.bin";

                try
                {
                    File.Copy(selectedFilePath, destinationFileName, true); 
                    Console.WriteLine($"Plik {selectedFilePath} został skopiowany do {destinationFileName}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                }

                RecordFile file = new RecordFile(destinationFileName, FileMode.OpenOrCreate);
                Sort(file);

                Console.WriteLine("Wciśnij dowolny klawisz, aby wrócić do menu.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór. Wciśnij dowolny klawisz, aby spróbować ponownie.");
                Console.ReadKey();
            }
        }
    }
    static void DisplayFile()
    {
        string directoryPath = Directory.GetCurrentDirectory();
        string[] files = Directory.GetFiles(directoryPath, "*.bin");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Lista plików:");

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
            }

            Console.WriteLine("0. Wyjście");

            Console.Write("Wybierz numer pliku lub 0 aby wyjść: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= files.Length)
            {
                if (choice == 0)
                {
                    Environment.Exit(0);
                }

                string selectedFilePath = files[choice - 1];
                Console.WriteLine($"Wybrano plik: {selectedFilePath}");

                RecordFile file = new RecordFile(selectedFilePath, FileMode.OpenOrCreate);
                file.Print();

                Console.WriteLine("Wciśnij dowolny klawisz, aby wrócić do menu.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór. Wciśnij dowolny klawisz, aby spróbować ponownie.");
                Console.ReadKey();
            }
        }
    }

    static void Sort(RecordFile file)
    {
        SortingEngine engine = new SortingEngine(file);
        var result = engine.Sort();
        engine.Clear();
        Console.Clear();
        result.Print();
        Console.ReadKey();
    }

}
