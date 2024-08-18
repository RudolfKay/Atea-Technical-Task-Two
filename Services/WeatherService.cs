using Atea.Task2.Interfaces;
using Atea.Task2.Models;
using System.Text.Json;

namespace Atea.Task2.Services;

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
