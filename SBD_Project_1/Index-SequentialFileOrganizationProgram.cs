using SBD_Project_1.Generation;
using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    internal class Index_SequentialFileOrganizationProgram
    {
        private FileService _fileService;
        public Index_SequentialFileOrganizationProgram(FileService fileService)
        {
            _fileService=fileService;
        }

        public void Start()
        {
            OperationMenu();
        }

        private void OperationMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("1. Insert Record");
                Console.WriteLine("2. Update Record");
                Console.WriteLine("3. Delete Record");
                Console.WriteLine("4. Get Record");
                Console.WriteLine("5. Display File");
                Console.WriteLine("6. Reorganize File");
                Console.WriteLine("7. Exit");
                Console.Write("Enter your choice (1-6): ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            InsertRecord();
                            break;
                        case 2:
                            UpdateRecord();
                            break;
                        case 3:
                            Console.Write("Enter key to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteKey))
                            {
                                DeleteRecord(deleteKey);
                            }
                            else
                            {
                                Console.WriteLine("Invalid key.");
                            }
                            break;
                        case 4:
                            Console.Write("Enter key to get: ");
                            if (int.TryParse(Console.ReadLine(), out int getKey))
                            {
                                GetRecord(getKey);
                            }
                            else
                            {
                                Console.WriteLine("Invalid key.");
                            }
                            break;
                        case 5:
                            DisplayFile();
                            break;
                        case 6:
                            Reorganize(); 
                            break;
                        case 7:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 6.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }

                Console.WriteLine();
            }
        }

        private void InsertRecord()
        {
            Console.Write("Enter key of record: ");
            if (int.TryParse(Console.ReadLine(), out int key))
            {
                RecordGenerator recordGeneration = new ManualInputRecordGenerator();
                Record newRecord = recordGeneration.GetRecord();

                Result result = _fileService.InsertRecord(key, newRecord);

                if(result.Type == ResultType.Success)
                {
                    Console.WriteLine("Record inserted successfully.");
                    Console.WriteLine(result);
                }
                else if (result.Type == ResultType.AlreadyExists)
                {
                    Console.WriteLine("Record already exists.");
                }
                else
                {
                    Console.WriteLine("Error inserting record.");
                }
            }
            else
            {
                Console.WriteLine("Invalid key.");
            }
        }
        private void UpdateRecord()
        {
            Console.Write("Enter key of updated record: ");
            if (int.TryParse(Console.ReadLine(), out int key))
            {
                Console.Write("Enter new key of record: ");
                if (int.TryParse(Console.ReadLine(), out int newKey))
                {
                    RecordGenerator recordGeneration = new ManualInputRecordGenerator();
                    Record newRecord = recordGeneration.GetRecord();

                    Result result = _fileService.UpdateRecord(key, newKey, newRecord);

                    if (result.Type == ResultType.Success)
                    {
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine("Record not found.");
                    }
                }
            }
        }

        private void DeleteRecord(int key)
        {
            Result result = _fileService.DeleteRecord(key);
            if (result.Type == ResultType.Success)
            {
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Record not found.");
            }
        }
        private void GetRecord(int key)
        {
            Result result = _fileService.GetRecord(key);
            if(result.Type == ResultType.Success)
            {
                Console.WriteLine(result);
                Console.WriteLine(result.Record);
            }
            else
            {
                Console.WriteLine("Record not found.");
            }
        }
        private void DisplayFile()
        {
            Result result = _fileService.DisplayFile();
            if (result.Type == ResultType.Success)
            {
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Error displaying file.");
            }
        }
        private void Reorganize()
        {
            Result result = _fileService.Reorganize();
            if (result.Type == ResultType.Success)
            {
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Error reorganizing file.");
            }
        }

    }
}
