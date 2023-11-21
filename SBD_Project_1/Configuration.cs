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
        ["bufferSize"] = $"{13*15*sizeof(int)}",
        ["tapesCount"] = "3"
    })
    .Build();

    }
}
