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

        // Inject the database context
        public WeatherController(WeatherDbContext context)
        {
            _context = context;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestWeatherData()
        {
            // This possibly needs to be a separate function. Needs to be refactored to take ALL data in database, group by city,
            // but return one record for each 6 cities where tempMin is all-time low temp and tempHigh is all-time high temp from all available data for that city
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
