using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using urbanmart.Models;
using urbanmart.Services;

public class NotificationJob
{
    private readonly NotificationsService _notificationsService;

    public NotificationJob(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    // This method will be triggered based on a timer
    [FunctionName("SendNotification")]
    public async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer, ILogger log)
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

        log.LogInformation($"Notification sent at: {DateTime.UtcNow}");
    }
}
