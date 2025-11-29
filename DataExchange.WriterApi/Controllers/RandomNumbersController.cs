using DataExchange.Shared.Models;
using DataExchange.WriterApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExchange.WriterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandomNumbersController : ControllerBase
    {
        private readonly ICsrngApiService _csrngApiService;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<RandomNumbersController> _logger;

        public RandomNumbersController(
            ICsrngApiService csrngApiService,
            IStorageApiClient storageApiClient,
            ILogger<RandomNumbersController> logger)
        {
            _csrngApiService = csrngApiService;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        /// <summary>
        /// Fetch random numbers from external API and send to Storage API
        /// </summary>
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchAndStoreNumbers([FromQuery] int count = 5, [FromQuery] int min = 1, [FromQuery] int max = 1000)
        {
            try
            {
                _logger.LogInformation("Fetching {Count} random numbers from CSRNG API", count);

                // 1. Fetch from external API
                var apiResponses = await _csrngApiService.GetRandomNumbersAsync(count, min, max);

                if (apiResponses.Count == 0)
                {
                    return BadRequest("Failed to fetch random numbers from external API");
                }

                // 2. Map to domain model
                var randomNumbers = apiResponses.Select(r => new RandomNumber
                {
                    Id = Guid.NewGuid(),
                    Value = r.Random,
                    Min = r.Min,
                    Max = r.Max,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                // 3. Send to Storage API
                var success = await _storageApiClient.StoreNumbersAsync(randomNumbers);

                if (!success)
                {
                    return StatusCode(500, "Failed to store numbers in Storage API");
                }

                _logger.LogInformation("Successfully fetched and stored {Count} random numbers", randomNumbers.Count);

                return Ok(new
                {
                    Message = $"Successfully fetched and stored {randomNumbers.Count} random numbers",
                    Numbers = randomNumbers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching and storing random numbers");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}