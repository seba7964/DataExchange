using DataExchange.Shared.Models;
using DataExchange.Storage.Implementations;
using FluentAssertions;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DataExchange.Storage.Tests
{
    public class InMemoryRandomNumberStorageTests : IDisposable
    {
        private readonly InMemoryRandomNumberStorage _storage;

        public InMemoryRandomNumberStorageTests()
        {
            _storage = new InMemoryRandomNumberStorage();
        }

        public void Dispose()
        {
            _storage.ClearAllAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddNumberAsync_ShouldAddNumberSuccessfully()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var number = new RandomNumber
            {
                Id = Guid.NewGuid(),
                Value = 42,
                Min = 1,
                Max = 100,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await _storage.AddNumberAsync(number);
            var result = await _storage.GetNumberByIdAsync(number.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(number.Id);
            result.Value.Should().Be(42);
        }

        [Fact]
        public async Task AddNumbersAsync_ShouldAddMultipleNumbers()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var numbers = new List<RandomNumber>
            {
                new RandomNumber { Id = Guid.NewGuid(), Value = 10, Min = 1, Max = 100, CreatedAt = DateTime.UtcNow },
                new RandomNumber { Id = Guid.NewGuid(), Value = 20, Min = 1, Max = 100, CreatedAt = DateTime.UtcNow },
                new RandomNumber { Id = Guid.NewGuid(), Value = 30, Min = 1, Max = 100, CreatedAt = DateTime.UtcNow }
            };

            // Act
            await _storage.AddNumbersAsync(numbers);
            var allNumbers = await _storage.GetAllNumbersAsync();

            // Assert
            allNumbers.Should().HaveCount(3);
            allNumbers.Select(n => n.Value).Should().Contain(new[] { 10, 20, 30 });
        }

        [Fact]
        public async Task GetNumberByIdAsync_WhenNumberDoesNotExist_ShouldReturnNull()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _storage.GetNumberByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllNumbersAsync_WhenEmpty_ShouldReturnEmptyList()
        {
            await _storage.ClearAllAsync();

            // Arrange & Act
            var result = await _storage.GetAllNumbersAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCountAsync_ShouldReturnCorrectCount()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var numbers = new List<RandomNumber>
            {
                new RandomNumber { Id = Guid.NewGuid(), Value = 1, Min = 1, Max = 10, CreatedAt = DateTime.UtcNow },
                new RandomNumber { Id = Guid.NewGuid(), Value = 2, Min = 1, Max = 10, CreatedAt = DateTime.UtcNow }
            };
            await _storage.AddNumbersAsync(numbers);

            // Act
            var count = await _storage.GetCountAsync();

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public async Task ClearAllAsync_ShouldRemoveAllNumbers()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var numbers = new List<RandomNumber>
            {
                new RandomNumber { Id = Guid.NewGuid(), Value = 1, Min = 1, Max = 10, CreatedAt = DateTime.UtcNow },
                new RandomNumber { Id = Guid.NewGuid(), Value = 2, Min = 1, Max = 10, CreatedAt = DateTime.UtcNow }
            };
            await _storage.AddNumbersAsync(numbers);

            // Act
            await _storage.ClearAllAsync();
            var count = await _storage.GetCountAsync();

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public async Task AddNumberAsync_WithDuplicateId_ShouldNotAddSecondNumber()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var id = Guid.NewGuid();
            var number1 = new RandomNumber { Id = id, Value = 100, Min = 1, Max = 100, CreatedAt = DateTime.UtcNow };
            var number2 = new RandomNumber { Id = id, Value = 200, Min = 1, Max = 100, CreatedAt = DateTime.UtcNow };

            // Act
            await _storage.AddNumberAsync(number1);
            await _storage.AddNumberAsync(number2);
            var result = await _storage.GetNumberByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(100);
        }

        [Fact]
        public async Task Storage_ShouldBeThreadSafe()
        {
            await _storage.ClearAllAsync();

            // Arrange
            var tasks = new List<Task>();
            var numberOfTasks = 100;

            // Act
            for (int i = 0; i < numberOfTasks; i++)
            {
                var localI = i;
                tasks.Add(Task.Run(async () =>
                {
                    var number = new RandomNumber
                    {
                        Id = Guid.NewGuid(),
                        Value = localI,
                        Min = 1,
                        Max = 1000,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _storage.AddNumberAsync(number);
                }));
            }

            await Task.WhenAll(tasks);
            var count = await _storage.GetCountAsync();

            // Assert
            count.Should().Be(numberOfTasks);
        }
    }
}