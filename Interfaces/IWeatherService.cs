namespace Atea.Task2.Interfaces;

public interface IWeatherService
{
    Task PollWeatherData(List<(string Country, string City, double Lat, double Lon)> locations, CancellationToken cancellationToken);
}
