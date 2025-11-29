using DataExchange.Shared.Models;

namespace DataExchange.WriterApi.Services
{
    public interface IStorageApiClient
    {
        Task<bool> StoreNumbersAsync(List<RandomNumber> numbers);
    }
}