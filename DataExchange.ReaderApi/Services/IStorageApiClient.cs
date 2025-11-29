using DataExchange.Shared.Models;

namespace DataExchange.ReaderApi.Services
{
    public interface IStorageApiClient
    {
        Task<List<RandomNumber>> GetAllNumbersAsync();
        Task<RandomNumber?> GetNumberByIdAsync(Guid id);
        Task<int> GetStatsAsync();
    }
}