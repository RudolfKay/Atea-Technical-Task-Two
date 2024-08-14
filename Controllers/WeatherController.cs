using Microsoft.AspNetCore.Mvc;
using Atea.Task2.Models;

namespace Atea.Task2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly string _filePath;

        public WeatherController(IConfiguration config)
        {
            _filePath = config["WeatherApi:DbFilePath"];
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestWeatherData()
        {
            try
            {
                // Read file content
                var lines = await System.IO.File.ReadAllLinesAsync(_filePath);

                // Ensure that we have enough lines to process
                if (lines.Length == 0)
                {
                    return NotFound("No weather data found.");
                }

                // Parse the lines into WeatherData objects and sort by timestamp
                var weatherDataList = lines
                    .Select(line =>
                    {
                        var parts = line.Split("||");

                        // Ensure that the line has the expected number of parts (in this case: 12 parts)
                        if (parts.Length < 12)
                        {
                            throw new FormatException("Weather data line is not in the expected format.");
                        }

                        return new WeatherData
                        {
                            Timestamp = DateTime.Parse(parts[0]),
                            Country = parts[1],
                            City = parts[2],
                            MinTemp = double.Parse(parts[4]),
                            MaxTemp = double.Parse(parts[6])
                        };
                    })
                    .OrderByDescending(data => data.Timestamp)  // Order by the latest timestamp
                    .Take(6)                                    // Take the top 6 most recent entries
                    .ToList();

                // Return the weather data list
                return Ok(weatherDataList);
            }
            catch (FormatException ex)
            {
                // Handle any issues with data parsing
                return BadRequest($"Data format error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors (e.g., file not found, etc.)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
