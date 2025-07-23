using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Services
{
    public class ExpiredUrlCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); 

        public ExpiredUrlCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    var expired = await db.UrlMappings
                        .Where(x => x.ExpirationDate != null && x.ExpirationDate < now)
                        .ToListAsync(stoppingToken);

                    if (expired.Any())
                    {
                        db.UrlMappings.RemoveRange(expired);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}
