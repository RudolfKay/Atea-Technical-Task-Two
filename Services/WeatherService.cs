using Atea.Task2.Interfaces;
using Atea.Task2.Models;
using System.Text.Json;

namespace Atea.Task2.Services;

/// <summary>
/// Service responsible for polling weather data from an external API and saving it to a database.
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly IWeatherRepository _weatherRepo;
    private readonly ILogger<WeatherService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _openWeatherUrl;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger, IWeatherRepository weatherRepo)
    {
        _openWeatherUrl = config["WeatherApi:OpenWeatherBaseUrl"];
        _weatherRepo = weatherRepo;
        _apiKey = config["WeatherApi:ApiKey"];
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Polls weather data for a list of locations and saves it to the database.
    /// </summary>
    /// <param name="locations">List of locations with country, city, latitude, and longitude.</param>
    /// <param name="cancellationToken">Cancellation token to allow operation cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// Retrieves weather data from the API for the specified latitude and longitude.
    /// </summary>
    /// <param name="lat">Latitude of the location.</param>
    /// <param name="lon">Longitude of the location.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="WeatherResponse"/> as the result.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="JsonException">Thrown when deserialization fails.</exception>
    private async Task<WeatherResponse> GetWeatherDataAsync(double lat, double lon)
    {
        var url = $"{_openWeatherUrl}lat={lat}&lon={lon}&appid={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error retrieving weather data: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<WeatherResponse>(jsonResponse) ?? throw new JsonException("Failed to deserialize weather data.");
    }

    /// <summary>
    /// Saves the retrieved weather data to the database.
    /// </summary>
    /// <param name="country">Country of the location.</param>
    /// <param name="city">City of the location.</param>
    /// <param name="weatherData">Weather data to be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

        await _weatherRepo.SaveWeatherDataAsync(weatherRecord);
    }
}
