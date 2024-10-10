using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using urbanmart.Models;
using urbanmart.Services;

namespace urbanmart.Services
{
    public class NotificationCronJobService : BackgroundService
    {
        private readonly ILogger<NotificationCronJobService> _logger;
        private readonly NotificationsService _notificationsService;

        public NotificationCronJobService(ILogger<NotificationCronJobService> logger, NotificationsService notificationsService)
        {
            _logger = logger;
            _notificationsService = notificationsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Create a new notification
                var notification = new Notification
                {
                    UserId = "1234",
                    Message = "This is a scheduled notification.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    Type = "General",
                    RelatedOrderId = null
                };

                // Post the notification to the database
                _notificationsService.Create(notification);

                _logger.LogInformation("Notification sent at: {time}", DateTimeOffset.Now);
                
                // Wait for 1 minute (60000 milliseconds)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
