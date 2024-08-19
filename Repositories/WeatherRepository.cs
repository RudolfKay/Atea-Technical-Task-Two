using Microsoft.EntityFrameworkCore;
using Atea.Task2.Interfaces;
using Atea.Task2.Context;
using Atea.Task2.Models;

namespace Atea.Task2.Repositories;

/// <summary>
/// Repository interface for managing weather data in the database.
/// </summary>
public class WeatherRepository : IWeatherRepository
{
    private readonly WeatherDbContext _context;

    public WeatherRepository(WeatherDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Saves a weather record to the database.
    /// </summary>
    /// <param name="weatherRecord">The weather record to be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveWeatherDataAsync(WeatherRecord weatherRecord)
    {
        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves the latest weather records from the database.
    /// </summary>
    /// <param name="count">The number of latest weather records to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, with a list of <see cref="WeatherRecord"/> as the result.</returns>
    public async Task<List<WeatherRecord>> GetLatestWeatherRecordsAsync(int count)
    {
        return await _context.WeatherRecords
            .OrderByDescending(w => w.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
