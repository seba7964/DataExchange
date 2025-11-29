using DataExchange.Shared.Models;

namespace DataExchange.ReaderApi.Models
{
    public class StorageApiResponse
    {
        public int TotalCount { get; set; }
        public List<RandomNumber> Numbers { get; set; } = new();
    }
}