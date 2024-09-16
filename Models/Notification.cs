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
