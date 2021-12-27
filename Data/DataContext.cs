using Microsoft.EntityFrameworkCore;
using UrlShortner.Models;

namespace UrlShortner.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<ShortUrl> ShortUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Keys
            
            
            // Columns
            modelBuilder.Entity<ShortUrl>().Property(t => t.LongUrl).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<ShortUrl>().Property(t => t.ShortenedUrl).IsRequired().HasMaxLength(500);

            // Relationships


        }
    }
}