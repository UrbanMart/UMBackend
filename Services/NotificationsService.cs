using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class NotificationsService
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationsService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _notifications = database.GetCollection<Notification>("Notifications");
        }

        // Get all notifications
        public List<Notification> Get() => _notifications.Find(notification => true).ToList();

        // Get a specific notification by ID
        public Notification Get(string id) => _notifications.Find(notification => notification.Id == id).FirstOrDefault();

        // Create a new notification
        public Notification Create(Notification notification)
        {
            _notifications.InsertOne(notification);
            return notification;
        }

        // Update an existing notification
        public void Update(string id, Notification updatedNotification) =>
            _notifications.ReplaceOne(notification => notification.Id == id, updatedNotification);

        // Mark a notification as read
        public void MarkAsRead(string id)
        {
            var notification = Get(id);
            if (notification != null)
            {
                notification.IsRead = true;
                Update(id, notification);
            }
        }

        // Delete a notification
        public void Delete(string id) =>
            _notifications.DeleteOne(notification => notification.Id == id);
    }
}
