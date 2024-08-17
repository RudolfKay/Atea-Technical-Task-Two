using Microsoft.Azure.WebJobs;
using Atea.Task2.Services;

namespace Atea.Task2.Functions
{
    public class WeatherPollingFunction
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherPollingFunction> _logger;

        private static readonly List<(string Country, string City, double Lat, double Lon)> Locations = new()
        {
            ("US", "New York", 40.7128, -74.0060),
            ("US", "Los Angeles", 34.0522, -118.2437),
            ("FR", "Paris", 48.8566, 2.3522),
            ("FR", "Marseille", 43.2965, 5.3698),
            ("GB", "London", 51.5074, -0.1278),
            ("GB", "Manchester", 53.4808, -2.2426)
        };

        public WeatherPollingFunction(IWeatherService weatherService, ILogger<WeatherPollingFunction> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        // Timer trigger function to poll weather data every minute
        [FunctionName("WeatherPollingFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo timer, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather polling function triggered at: {time}", DateTime.Now);

            try
            {
                // Poll and store weather data
                await _weatherService.PollWeatherData(Locations, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while polling weather data.");
            }
        }
    }
}
