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
       public static IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>()
    {
        ["maxRecordLength"] = "15",
        ["maxNumberInRecord"] = "100",
        ["bufferSize"] = $"{2*15*sizeof(int)}", //max 13 records in buffer
        ["recordsCount"] = "13",
        ["tapesCount"] = "3"
    })
    .Build();


        public static int MAX_RECORD_LENGTH = 15;
        public static int MAX_NUMBER_IN_RECORD = 100;
        public static int BUFFER_SIZE = 2 * MAX_RECORD_LENGTH * sizeof(int);
        public static int RECORDS_COUNT = 13;
        public static int TAPES_COUNT = 3;
    }
}
