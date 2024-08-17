using Atea.Task2.Context;
using Atea.Task2.Models;
using System.Text.Json;

namespace Atea.Task2.Services;

public interface IWeatherService
{
    Task PollWeatherData(List<(string Country, string City, double Lat, double Lon)> locations, CancellationToken cancellationToken);
}

/// <summary>
/// Provides services for retrieving and storing weather data.
/// Implements the <see cref="IWeatherService"/> interface.
/// </summary>
/// <remarks>
/// The <see cref="WeatherService"/> class interacts with the OpenWeatherMap API to fetch weather data for specified locations.
/// It then stores the retrieved weather data into a database. This service is designed to handle multiple locations and perform 
/// data storage operations asynchronously. It uses dependency injection to receive configuration settings, logging services, and
/// the database context for data access.
/// </remarks>
public class WeatherService : IWeatherService
{
    private readonly string _apiKey;
    private readonly ILogger<WeatherService> _logger;
    private readonly WeatherDbContext _context;

    public WeatherService(IConfiguration config, ILogger<WeatherService> logger, WeatherDbContext context)
    {
        _apiKey = config["WeatherApi:ApiKey"];
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Polls weather data for a list of locations and stores it in the database.
    /// </summary>
    /// <param name="locations">A list of locations including Country, City, Latitude, and Longitude for weather polling.</param>
    /// <param name="cancellationToken">A cancellation token to monitor for request cancellation.</param>
    /// <remarks>
    /// This method iterates over the given locations, retrieves weather data for each, and stores the data in the database.
    /// If the cancellation token is triggered, the polling operation will stop.
    /// In case of any failure in fetching or saving the weather data, an error is logged.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PollWeatherData(List<(string Country, string City, double Lat, double Lon)> locations, CancellationToken cancellationToken)
    {
        foreach (var location in locations)
        {
            if (cancellationToken.IsCancellationRequested) break;

            try
            {
                var weatherData = await GetWeatherDataAsync(location.Lat, location.Lon);
                await SaveWeatherDataToDatabaseAsync(location.Country, location.City, weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data for {location.City}, {location.Country}: {ex}");
            }
        }
    }

    /// <summary>
    /// Fetches weather data from the OpenWeatherMap API for a specific location.
    /// </summary>
    /// <param name="lat">Latitude of the location.</param>
    /// <param name="lon">Longitude of the location.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <see cref="WeatherResponse"/> containing the weather data.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails or returns an unsuccessful status code.</exception>
    /// <exception cref="JsonException">Thrown when the weather data cannot be deserialized from the API response.</exception>
    private async Task<WeatherResponse> GetWeatherDataAsync(double lat, double lon)
    {
        using var httpClient = new HttpClient();
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error retrieving weather data: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<WeatherResponse>(jsonResponse) ?? throw new JsonException("Failed to deserialize weather data.");
    }

    /// <summary>
    /// Saves the weather data for a specific location to the database.
    /// </summary>
    /// <param name="country">The country where the city is located.</param>
    /// <param name="city">The city for which weather data is being saved.</param>
    /// <param name="weatherData">The weather data to be saved, including temperature and humidity.</param>
    /// <remarks>
    /// If the weather data's 'Main' section is missing, the data will not be saved, and a warning will be logged.
    /// </remarks>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    private async Task SaveWeatherDataToDatabaseAsync(string country, string city, WeatherResponse weatherData)
    {
        if (weatherData?.Main == null)
        {
            _logger.LogWarning($"Weather data is missing 'Main' section for {city}, {country}.");
            return;
        }

        var weatherRecord = new WeatherRecord
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Country = country,
            City = city,
            MinTemp = weatherData.Main.TempMin,
            MaxTemp = weatherData.Main.TempMax,
            CurrentTemp = weatherData.Main.Temp,
            Humidity = weatherData.Main.Humidity
        };

        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();
    }
}
