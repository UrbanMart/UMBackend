using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class NotificationCronJobService : IHostedService, IDisposable
    {
        private Timer _timer;
        private Timer _pollingTimer; // New polling timer
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationCronJobService> _logger;
        private readonly IMongoCollection<CronJobSetting> _cronJobSettingsCollection;
        private int _intervalInMinutes;

        public NotificationCronJobService(IServiceProvider serviceProvider, ILogger<NotificationCronJobService> logger, IDatabaseSettings settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Initialize MongoDB collection for CronJobSettings
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _cronJobSettingsCollection = database.GetCollection<CronJobSetting>("CronJobSettings");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NotificationCronJobService started at {time}", DateTime.Now);

            // Fetch the interval from the database
            await FetchIntervalFromDatabaseAsync();

            // If interval is fetched successfully, start the timer
            if (_intervalInMinutes > 0)
            {
                var interval = TimeSpan.FromMinutes(_intervalInMinutes);
                _timer = new Timer(DoWork, null, TimeSpan.Zero, interval);
                
                // Start the polling timer to check for updates
                _pollingTimer = new Timer(CheckForIntervalUpdates, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1)); // Poll every minute
            }
            else
            {
                _logger.LogWarning("Failed to fetch a valid interval from the database. Timer will not start.");
            }
        }

        private async Task FetchIntervalFromDatabaseAsync()
        {
            try
            {
                // Fetch the CronJobSetting document with the specific name "NotificationCronJob"
                var cronJobSetting = await _cronJobSettingsCollection
                    .Find(setting => setting.Name == "NotificationCronJob")
                    .FirstOrDefaultAsync();

                if (cronJobSetting != null)
                {
                    _intervalInMinutes = cronJobSetting.IntervalInMinutes;
                    _logger.LogInformation("Fetched interval of {intervalInMinutes} minutes from the database for cron job '{name}'.",
                        _intervalInMinutes, cronJobSetting.Name);
                }
                else
                {
                    _logger.LogWarning("No cron job settings found in the database for 'NotificationCronJob'.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching interval from database.");
            }
        }

        private async void CheckForIntervalUpdates(object state)
        {
            var cronJobSetting = await _cronJobSettingsCollection
                .Find(setting => setting.Name == "NotificationCronJob")
                .FirstOrDefaultAsync();

            if (cronJobSetting != null && cronJobSetting.IntervalInMinutes != _intervalInMinutes)
            {
                _logger.LogInformation("Cron job interval updated in the database. Restarting timer...");
                RestartTimer(cronJobSetting.IntervalInMinutes);
            }
        }

        private void RestartTimer(int newIntervalInMinutes)
        {
            _intervalInMinutes = newIntervalInMinutes;
            var interval = TimeSpan.FromMinutes(_intervalInMinutes);
            _timer?.Change(Timeout.Infinite, 0); // Stop the old timer
            _timer = new Timer(DoWork, null, TimeSpan.Zero, interval); // Start a new timer
            _logger.LogInformation("Timer restarted with new interval of {intervalInMinutes} minutes.", _intervalInMinutes);
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
            _pollingTimer?.Change(Timeout.Infinite, 0); // Stop the polling timer
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _pollingTimer?.Dispose(); // Dispose of the polling timer
        }
    }
}
