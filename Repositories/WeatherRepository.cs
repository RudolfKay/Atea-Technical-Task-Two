using Atea.Task2.Interfaces;
using Atea.Task2.Context;
using Atea.Task2.Models;
using Microsoft.EntityFrameworkCore;

namespace Atea.Task2.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly WeatherDbContext _context;

    public WeatherRepository(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task SaveWeatherDataAsync(WeatherRecord weatherRecord)
    {
        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WeatherRecord>> GetLatestWeatherRecordsAsync(int count)
    {
        return await _context.WeatherRecords
            .OrderByDescending(w => w.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
