namespace Atea.Task2.Services;

public class WeatherPollingJob : BackgroundService
{
    private readonly ILogger<WeatherPollingJob> _logger;
    private readonly WeatherService _weatherService;
    private static readonly List<(string Country, string City, double Lat, double Lon)> locations = new()
    {
        ("US", "New York", 40.7128, -74.0060),
        ("US", "Los Angeles", 34.0522, -118.2437),
        ("FR", "Paris", 48.8566, 2.3522),
        ("FR", "Marseille", 43.2965, 5.3698),
        ("GB", "London", 51.5074, -0.1278),
        ("GB", "Manchester", 53.4808, -2.2426)
    };

    public WeatherPollingJob(ILogger<WeatherPollingJob> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Weather polling service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Polling weather data...");
            
            // Pass the CancellationToken to the PollWeatherData method
            await _weatherService.PollWeatherData(locations, stoppingToken);

            // Wait for 1 minute before polling again, check cancellation token
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Weather polling task was canceled.");
            }
        }

        _logger.LogInformation("Weather polling service is stopping.");
    }
}
