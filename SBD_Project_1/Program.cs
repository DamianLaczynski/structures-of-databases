using SBD_Project_1;
using SBD_Project_1.Models;
using SBD_Project_1.PoliphaseSort;
using SBD_Project_1.RecordGeneration;



//SortingProgram sortingProgram = new SortingProgram();
//sortingProgram.Start();
Index_SequentialFileOrganizationProgram indexSequentialFileOrganizationProgram = new Index_SequentialFileOrganizationProgram(new FileService());
indexSequentialFileOrganizationProgram.Start();
