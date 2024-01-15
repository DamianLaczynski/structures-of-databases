using Microsoft.Extensions.Configuration;
using SBD_Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBD_Project_1
{
    public static class Configuration
    {
        public static readonly int MAX_RECORD_LENGTH = 15;
        public static readonly int MAX_NUMBER_IN_RECORD = 100;
        public static readonly int BUFFER_SIZE = 5 * MAX_RECORD_LENGTH * sizeof(int);
        public static readonly int RECORDS_COUNT = 1000;
        public static readonly int TAPES_COUNT = 3;
        public static readonly int MAX_RECORDS_IN_BUFFER = BUFFER_SIZE / (15 * sizeof(int));

        public static class FileOrganization
        {
            public static readonly int PageSize = 4 * ((15 * sizeof(int)) + (2*sizeof(int))); //4 records in buffer
            public static readonly int PrimaryAreaRecordsCount = 4;
            public static readonly int OverflowAreaRecordsCount = 4;
            public static readonly int IndexRecordsCount = PageSize / (2*sizeof(int));
            public static readonly double PageUtilization = 0.75;
            public static readonly double OverflowToPrimary = 0.2;
        }
    }
}
