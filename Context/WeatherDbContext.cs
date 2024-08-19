using Microsoft.EntityFrameworkCore;
using Atea.Task2.Models;

#nullable disable

namespace Atea.Task2.Context
{
    /// <summary>
    /// Represents the database context for weather data.
    /// </summary>
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the collection of weather records in the database.
        /// </summary>
        public DbSet<WeatherRecord> WeatherRecords { get; set; }

        /// <summary>
        /// Configures the model and its relationships for the weather records.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherRecord>()
                .ToTable("WeatherRecords")
                .HasKey(wr => wr.Id);

            modelBuilder.Entity<WeatherRecord>()
                .Property(wr => wr.Country)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<WeatherRecord>()
                .Property(wr => wr.Timestamp)
                .HasColumnType("datetime");
        }
    }
}
