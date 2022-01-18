using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Models;
using UrlShortner.Models.Auth;

namespace UrlShortner.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<ShortUrl> ShortUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Keys
            
            
            // Columns
            modelBuilder.Entity<ShortUrl>().Property(t => t.LongUrl).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<ShortUrl>().Property(t => t.ShortenedUrl).IsRequired().HasMaxLength(500);

            // Relationships


        }
    }
}