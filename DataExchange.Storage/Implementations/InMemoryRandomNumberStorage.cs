using System.Collections.Concurrent;
using DataExchange.Shared.Models;
using DataExchange.Storage.Interfaces;

namespace DataExchange.Storage.Implementations
{
    public class InMemoryRandomNumberStorage : IRandomNumberStorage
    {
        private static readonly ConcurrentDictionary<Guid, RandomNumber> _numbers = new();

        public Task AddNumberAsync(RandomNumber number)
        {
            _numbers.TryAdd(number.Id, number);
            return Task.CompletedTask;
        }

        public Task AddNumbersAsync(IEnumerable<RandomNumber> numbers)
        {
            foreach (var number in numbers)
            {
                _numbers.TryAdd(number.Id, number);
            }
            return Task.CompletedTask;
        }

        public Task<RandomNumber?> GetNumberByIdAsync(Guid id)
        {
            _numbers.TryGetValue(id, out var number);
            return Task.FromResult(number);
        }

        public Task<IEnumerable<RandomNumber>> GetAllNumbersAsync()
        {
            return Task.FromResult<IEnumerable<RandomNumber>>(_numbers.Values.ToList());
        }

        public Task<int> GetCountAsync()
        {
            return Task.FromResult(_numbers.Count);
        }

        public Task ClearAllAsync()
        {
            _numbers.Clear();
            return Task.CompletedTask;
        }
    }
}