using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Atea.Task2.Context;

namespace Atea.Task2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherDbContext _context;

        public WeatherController(WeatherDbContext context)
        {
            _context = context;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestWeatherData()
        {
            try
            {
                // Retrieve the latest 6 weather data entries, ordered by timestamp (descending)
                var weatherDataList = await _context.WeatherRecords
                    .OrderByDescending(w => w.Timestamp)
                    .Take(6)
                    .ToListAsync();

                // Ensure that there is weather data in the database
                if (weatherDataList == null || !weatherDataList.Any())
                {
                    return NotFound("No weather data found.");
                }

                // Return the latest 6 weather data entries
                return Ok(weatherDataList);
            }
            catch (Exception ex)
            {
                // Handle general errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
