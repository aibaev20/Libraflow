using System;
using System.Threading;
using System.Threading.Tasks;
using BookDepoSystem.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookDepoSystem.Services.Implementations;

public class RentStatusBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly TimeSpan updateInterval = TimeSpan.FromSeconds(10);

    public RentStatusBackgroundService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var rentService = scope.ServiceProvider.GetRequiredService<IRentService>();

                await rentService.UpdateAllRentStatusesAsync();
            }

            await Task.Delay(this.updateInterval, stoppingToken);
        }
    }
}