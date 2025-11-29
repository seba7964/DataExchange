using DataExchange.Shared.Models;
using System.Text;
using System.Text.Json;

namespace DataExchange.WriterApi.Services
{
    public class StorageApiClient : IStorageApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StorageApiClient> _logger;
        private readonly string _storageApiBaseUrl;

        public StorageApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<StorageApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _storageApiBaseUrl = configuration["StorageApi:BaseUrl"] ?? "https://localhost:7000";
        }

        public async Task<bool> StoreNumbersAsync(List<RandomNumber> numbers)
        {
            try
            {
                var json = JsonSerializer.Serialize(numbers);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_storageApiBaseUrl}/api/storage/numbers", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully stored {Count} numbers in Storage API", numbers.Count);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to store numbers. Status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Storage API");
                return false;
            }
        }
    }
}