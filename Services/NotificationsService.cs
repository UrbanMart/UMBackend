/*
 * File: NotificationsService.cs
 * Description: This file defines the NotificationsService class for the UrbanMart application, 
 *              which provides methods for managing notification-related operations in the 
 *              MongoDB database, such as creating, retrieving, updating, and deleting notifications.
 * Author: Dinithi Mendis
 * Date: 16/09/24
 * 
 * The NotificationsService class interacts with the MongoDB collection for notification entities 
 * and provides CRUD functionality for notifications.
 */

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

        // Retrieve all notifications
        public List<Notification> Get() => _notifications.Find(notification => true).ToList();

        // Retrieve notifications by UserId
        public List<Notification> GetByUserId(string userId)
        {
            // Fetch notifications where the UserId matches the provided userId
            return _notifications.Find(notification => notification.UserId == userId).ToList();
        }

        // Retrieve a notification by its ID
        public Notification Get(string id) => _notifications.Find(notification => notification.Id == id).FirstOrDefault();

        // Create a new notification in the system
        public Notification Create(Notification notification)
        {
            _notifications.InsertOne(notification);
            return notification;
        }

        // Update an existing notification
        public void Update(string id, Notification updatedNotification) =>
            _notifications.ReplaceOne(notification => notification.Id == id, updatedNotification);

        // Delete a notification by ID
        public void Delete(string id) =>
            _notifications.DeleteOne(notification => notification.Id == id);
    }
}
