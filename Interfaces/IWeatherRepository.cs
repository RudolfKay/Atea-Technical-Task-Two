using Atea.Task2.Models;

namespace Atea.Task2.Interfaces;

public interface IWeatherRepository
{
    Task SaveWeatherDataAsync(WeatherRecord weatherRecord);
    Task<List<WeatherRecord>> GetLatestWeatherRecordsAsync(int count);
}
