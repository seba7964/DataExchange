using DataExchange.Shared.Models;
using System.Text.Json;

namespace DataExchange.WriterApi.Services
{
    public class CsrngApiService : ICsrngApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CsrngApiService> _logger;
        private readonly string _baseUrl;
        private readonly int _delayBetweenRequestsMs;
        private readonly int _timeoutSeconds;

        public CsrngApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CsrngApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _baseUrl = configuration["CsrngApi:BaseUrl"] ?? "https://csrng.net/csrng/csrng.php";
            _delayBetweenRequestsMs = configuration.GetValue<int>("CsrngApi:DelayBetweenRequestsMs", 1000);
            _timeoutSeconds = configuration.GetValue<int>("CsrngApi:TimeoutSeconds", 10);

            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        public async Task<List<CsrngResponse>> GetRandomNumbersAsync(int count, int min = 1, int max = 1000)
        {
            var results = new List<CsrngResponse>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var url = $"{_baseUrl}?min={min}&max={max}";

                    _logger.LogInformation("Fetching random number {Current}/{Total} from CSRNG API", i + 1, count);

                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<List<CsrngResponse>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Count > 0)
                    {
                        results.Add(apiResponse[0]);
                        _logger.LogInformation("Successfully fetched number: {Value}", apiResponse[0].Random);
                    }

                    if (i < count - 1)
                    {
                        _logger.LogDebug("Waiting {Delay}ms before next request", _delayBetweenRequestsMs);
                        await Task.Delay(_delayBetweenRequestsMs);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "HTTP error fetching random number {Current}/{Total}", i + 1, count);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex, "Request timeout fetching random number {Current}/{Total}", i + 1, count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error fetching random number {Current}/{Total}", i + 1, count);
                }
            }

            return results;
        }
    }
}