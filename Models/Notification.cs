/*
 * File: Notification.cs
 * Description: This file defines the Notification model for the UrbanMart application.
 *              It represents notifications that can be sent to users, including details such 
 *              as the notification message, status, timestamp, and related data.
 * Author: Dinithi Mendis
 * Date:16/09/24
 * 
 * The Notification model is used to manage user notifications within the system, 
 * and is stored in a MongoDB collection.
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace urbanmart.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; } // Reference to the user receiving the notification

        [BsonElement("Message")]
        public string Message { get; set; } // Notification message

        [BsonElement("IsRead")]
        public bool IsRead { get; set; } = false; // Indicates if the notification has been read

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of notification creation

        [BsonElement("Type")]
        public string Type { get; set; } // Type of notification (e.g., "Order", "Cancellation", "Account Approval")
        
        // Optional: You can add a property to include additional data or reference IDs if needed
        [BsonElement("RelatedOrderId")]
        public string RelatedOrderId { get; set; } // Reference to the related order, if applicable
    }
}
