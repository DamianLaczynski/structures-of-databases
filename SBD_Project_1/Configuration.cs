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
        public static readonly int BUFFER_SIZE = 3 * MAX_RECORD_LENGTH * sizeof(int);
        public static readonly int RECORDS_COUNT = 1000;
        public static readonly int TAPES_COUNT = 3;
        public static readonly int MAX_RECORDS_IN_BUFFER = BUFFER_SIZE/(15*sizeof(int));
    }
}
