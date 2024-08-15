using Atea.Task2.Context;
using Atea.Task2.Models;
using System.Text.Json;

namespace Atea.Task2.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<WeatherService> _logger;
        private readonly WeatherDbContext _context;

        public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger, WeatherDbContext context)
        {
            _httpClient = httpClient;
            _apiKey = config["WeatherApi:ApiKey"];
            _logger = logger;
            _context = context;
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
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";
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
}
