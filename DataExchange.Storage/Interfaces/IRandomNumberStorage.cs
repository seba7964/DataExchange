using DataExchange.Shared.Models;

namespace DataExchange.Storage.Interfaces
{
    public interface IRandomNumberStorage
    {
        Task AddNumberAsync(RandomNumber number);
        Task AddNumbersAsync(IEnumerable<RandomNumber> numbers);
        Task<RandomNumber?> GetNumberByIdAsync(Guid id);
        Task<IEnumerable<RandomNumber>> GetAllNumbersAsync();
        Task<int> GetCountAsync();
        Task ClearAllAsync();
    }
}