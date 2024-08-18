using Microsoft.EntityFrameworkCore;
using Atea.Task2.Interfaces;
using Atea.Task2.Services;
using Atea.Task2.Context;
using Atea.Task2.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WeatherDatabase")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddHostedService<WeatherPollingJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
