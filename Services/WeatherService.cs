using Atea.Task2.Models;
using System.Text.Json;

namespace Atea.Task2.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _apiKey;
    private readonly string _filePath;

    public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        _apiKey = config["WeatherApi:ApiKey"];
        _filePath = config["WeatherApi:DbFilePath"];
    }

    public async Task PollWeatherData(List<(string Country, string City, double Lat, double Lon)> locations, CancellationToken cancellationToken)
    {
        foreach (var location in locations)
        {
            if (cancellationToken.IsCancellationRequested) break;

            try
            {
                var weatherData = await GetWeatherDataAsync(location.Lat, location.Lon);
                await SaveWeatherDataToFileAsync(location.Country, location.City, weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving data for {location.City}, {location.Country}");
            }
        }
    }

    private async Task<WeatherResponse> GetWeatherDataAsync(double lat, double lon)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error retrieving weather data: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<WeatherResponse>(jsonResponse) ?? throw new JsonException("Failed to deserialize weather data.");
    }

    private async Task SaveWeatherDataToFileAsync(string country, string city, WeatherResponse weatherData)
    {
        if (weatherData?.Main == null)
        {
            _logger.LogWarning($"Weather data is missing 'Main' section for {city}, {country}.");
            return;
        }

        var logEntry = $"{DateTime.UtcNow:o}||{country}||{city}||Min Temp:||{weatherData.Main.TempMin}||Max Temp:||{weatherData.Main.TempMax}||Current Temp:||{weatherData.Main.Temp}||Humidity:||{weatherData.Main.Humidity}||\n";
        await File.AppendAllTextAsync(_filePath, logEntry);
    } // 0 1 2 4 6 8 10
}
