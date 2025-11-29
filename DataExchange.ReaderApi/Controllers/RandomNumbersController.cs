using DataExchange.ReaderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExchange.ReaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandomNumbersController : ControllerBase
    {
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<RandomNumbersController> logger;

        public RandomNumbersController(
            IStorageApiClient storageApiClient,
            ILogger<RandomNumbersController> logger)
        {
            _storageApiClient = storageApiClient;
            this.logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var numbers = await _storageApiClient.GetAllNumbersAsync();

                logger.LogInformation("Retrieved {Count} random numbers", numbers.Count);

                return Ok(new
                {
                    TotalCount = numbers.Count,
                    Numbers = numbers.OrderByDescending(n => n.CreatedAt)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all numbers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var number = await _storageApiClient.GetNumberByIdAsync(id);

                if (number == null)
                {
                    logger.LogWarning("Number with ID {Id} not found", id);
                    return NotFound(new { Message = $"Number with ID {id} not found" });
                }

                logger.LogInformation("Retrieved number with ID {Id}", id);
                return Ok(number);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving number by ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var numbers = await _storageApiClient.GetAllNumbersAsync();

                if (numbers.Count == 0)
                {
                    return Ok(new
                    {
                        TotalCount = 0,
                        Message = "No numbers stored yet"
                    });
                }

                var stats = new
                {
                    TotalCount = numbers.Count,
                    MinValue = numbers.Min(n => n.Value),
                    MaxValue = numbers.Max(n => n.Value),
                    AverageValue = numbers.Average(n => n.Value),
                    OldestEntry = numbers.Min(n => n.CreatedAt),
                    NewestEntry = numbers.Max(n => n.CreatedAt)
                };

                logger.LogInformation("Retrieved statistics for {Count} numbers", numbers.Count);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating stats");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? minValue, [FromQuery] int? maxValue)
        {
            try
            {
                var allNumbers = await _storageApiClient.GetAllNumbersAsync();
                var filtered = allNumbers.AsEnumerable();

                if (minValue.HasValue)
                {
                    filtered = filtered.Where(n => n.Value >= minValue.Value);
                }

                if (maxValue.HasValue)
                {
                    filtered = filtered.Where(n => n.Value <= maxValue.Value);
                }

                var result = filtered.OrderByDescending(n => n.CreatedAt).ToList();

                logger.LogInformation("Search returned {Count} numbers (min: {Min}, max: {Max})",
                    result.Count, minValue, maxValue);

                return Ok(new
                {
                    Count = result.Count,
                    SearchCriteria = new { MinValue = minValue, MaxValue = maxValue },
                    Numbers = result
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching numbers");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}