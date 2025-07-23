using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;


namespace UrlShortener.Data
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UrlMapping> UrlMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMapping>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();

            modelBuilder.Entity<UrlMapping>()
                .HasIndex(u => u.OriginalUrl)
                .IsUnique();
        }
    }

}
