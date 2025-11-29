using DataExchange.ReaderApi.Models;
using DataExchange.Shared.Models;
using System.Text.Json;

namespace DataExchange.ReaderApi.Services
{
    public class StorageApiClient : IStorageApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StorageApiClient> _logger;
        private readonly string _storageApiBaseUrl;
        private readonly int _timeoutSeconds;

        public StorageApiClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<StorageApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _storageApiBaseUrl = configuration["StorageApi:BaseUrl"] ?? "https://localhost:7000";
            _timeoutSeconds = configuration.GetValue<int>("StorageApi:TimeoutSeconds", 30);

            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        public async Task<List<RandomNumber>> GetAllNumbersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_storageApiBaseUrl}/api/storage/numbers");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StorageApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("Retrieved {Count} numbers from Storage API", result?.Numbers?.Count ?? 0);
                    return result?.Numbers ?? new List<RandomNumber>();
                }
                else
                {
                    _logger.LogError("Failed to retrieve numbers. Status: {StatusCode}", response.StatusCode);
                    return new List<RandomNumber>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Storage API");
                return new List<RandomNumber>();
            }
        }

        public async Task<RandomNumber?> GetNumberByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_storageApiBaseUrl}/api/storage/numbers/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var number = JsonSerializer.Deserialize<RandomNumber>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("Retrieved number with ID {Id} from Storage API", id);
                    return number;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Number with ID {Id} not found", id);
                    return null;
                }
                else
                {
                    _logger.LogError("Failed to retrieve number by ID. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Storage API for number {Id}", id);
                return null;
            }
        }

        public async Task<int> GetStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_storageApiBaseUrl}/api/storage/stats");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StatsResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result?.TotalNumbers ?? 0;
                }
                else
                {
                    _logger.LogError("Failed to retrieve stats. Status: {StatusCode}", response.StatusCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Storage API for stats");
                return 0;
            }
        }
    }
}