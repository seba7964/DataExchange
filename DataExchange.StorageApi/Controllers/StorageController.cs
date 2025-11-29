using DataExchange.Shared.Models;
using DataExchange.Storage.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataExchange.StorageApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IRandomNumberStorage _storage;
        private readonly ILogger<StorageController> _logger;

        public StorageController(IRandomNumberStorage storage, ILogger<StorageController> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        /// <summary>
        /// Writer API će zvati ovaj endpoint da spremi brojeve
        /// </summary>
        [HttpPost("numbers")]
        public async Task<IActionResult> StoreNumbers([FromBody] List<RandomNumber> numbers)
        {
            try
            {
                if (numbers == null || numbers.Count == 0)
                {
                    return BadRequest("No numbers provided");
                }

                await _storage.AddNumbersAsync(numbers);
                _logger.LogInformation("Stored {Count} numbers", numbers.Count);

                return Ok(new
                {
                    Message = $"Successfully stored {numbers.Count} numbers",
                    Count = numbers.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing numbers");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Reader API će zvati ovaj endpoint da dohvati sve brojeve
        /// </summary>
        [HttpGet("numbers")]
        public async Task<IActionResult> GetAllNumbers()
        {
            try
            {
                var numbers = await _storage.GetAllNumbersAsync();
                var numbersList = numbers.ToList();

                _logger.LogInformation("Retrieved {Count} numbers", numbersList.Count);

                return Ok(new
                {
                    TotalCount = numbersList.Count,
                    Numbers = numbersList.OrderByDescending(n => n.CreatedAt)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving numbers");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Dohvati broj po ID-u
        /// </summary>
        [HttpGet("numbers/{id}")]
        public async Task<IActionResult> GetNumberById(Guid id)
        {
            try
            {
                var number = await _storage.GetNumberByIdAsync(id);

                if (number == null)
                {
                    return NotFound(new { Message = $"Number with ID {id} not found" });
                }

                return Ok(number);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving number by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Statistika
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var count = await _storage.GetCountAsync();
                return Ok(new
                {
                    TotalNumbers = count,
                    Message = $"Currently {count} random numbers in storage"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stats");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obriši sve
        /// </summary>
        [HttpDelete("numbers")]
        public async Task<IActionResult> ClearAll()
        {
            try
            {
                await _storage.ClearAllAsync();
                _logger.LogInformation("Cleared all numbers");
                return Ok(new { Message = "All numbers cleared" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing storage");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}