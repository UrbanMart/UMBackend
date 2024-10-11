using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace urbanmart.Services
{
    public class NotificationCronJobService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationCronJobService> _logger;

        public NotificationCronJobService(IServiceProvider serviceProvider, ILogger<NotificationCronJobService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NotificationCronJobService started at {time}", DateTime.Now);

            // Run every 1 minute
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("Inventory check task started at {time}", DateTime.Now);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryCheckService = scope.ServiceProvider.GetRequiredService<InventoryCheckService>();
                    inventoryCheckService.CheckAndProcessOrders();
                }

                _logger.LogInformation("Inventory check task completed at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during inventory check task at {time}", DateTime.Now);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NotificationCronJobService stopped at {time}", DateTime.Now);

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
