using Atea.Task2.Interfaces;

namespace Atea.Task2.Services;

/// <summary>
/// A background service that periodically polls weather data for a predefined list of locations.
/// </summary>
/// <remarks>
/// This service runs in the background and triggers every minute to fetch and store weather data.
/// It uses the <see cref="IWeatherService"/> to perform the polling and data storage.
/// </remarks>
public class WeatherPollingJob : BackgroundService
{
    public IServiceProvider Services { get; }
    private readonly ILogger<WeatherPollingJob> _logger;
    private static readonly List<(string Country, string City, double Lat, double Lon)> locations = new()
    {
        ("US", "New York", 40.7128, -74.0060),
        ("US", "Los Angeles", 34.0522, -118.2437),
        ("FR", "Paris", 48.8566, 2.3522),
        ("FR", "Marseille", 43.2965, 5.3698),
        ("GB", "London", 51.5074, -0.1278),
        ("GB", "Manchester", 53.4808, -2.2426)
    };

    public WeatherPollingJob(IServiceProvider services, ILogger<WeatherPollingJob> logger)
    {
        Services = services;
        _logger = logger;
    }

    /// <summary>
    /// Executes the background service that polls weather data periodically.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to monitor for service stopping.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Weather polling service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Polling weather data...");
            
            var weatherService = Services.CreateScope().ServiceProvider
                                         .GetRequiredService<IWeatherService>();

            await weatherService.PollWeatherData(locations, stoppingToken);

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
