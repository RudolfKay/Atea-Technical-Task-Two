using Microsoft.AspNetCore.Mvc;
using Atea.Task2.Interfaces;

namespace Atea.Task2.Controllers;

/// <summary>
/// Controller for managing weather data endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepo;

    public WeatherController(IWeatherRepository weatherRepo)
    {
        _weatherRepo = weatherRepo;
    }

    /// <summary>
    /// Retrieves the latest 6 weather records from the database.
    /// </summary>
    /// <remarks>
    /// Queries the database for the most recent 6 weather data entries, ordered by timestamp in descending order.
    /// </remarks>
    /// <returns>
    /// A list of the latest 6 weather records, or an appropriate HTTP error response.
    /// </returns>
    /// <response code="200">Returns the latest 6 weather data entries.</response>
    /// <response code="404">If no weather data is found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestWeatherData()
    {
        try
        {
            // Retrieve the latest 6 weather data entries, ordered by timestamp (descending)
            var weatherDataList = await _weatherRepo.GetLatestWeatherRecordsAsync(6);

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
