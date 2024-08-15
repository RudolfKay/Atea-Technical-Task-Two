using Microsoft.EntityFrameworkCore;
using Atea.Task2.Models;

namespace Atea.Task2.Context
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<WeatherRecord> WeatherRecords { get; set; }

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
