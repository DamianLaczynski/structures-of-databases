using SBD_Project_1.Models;

namespace SBD_Project_1.Generation
{
    internal interface IRecordGenerator<T> where T : class
    {
        T GetRecord() ;
        List<T> GetRecords(int count);
    }
}