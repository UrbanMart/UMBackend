using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace urbanmart.Models
{
    public class VendorFeedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("VendorId")]
        public string VendorId { get; set; } // The vendor being reviewed

        [BsonElement("CustomerId")]
        public string CustomerId { get; set; } // The customer who left the feedback

        [BsonElement("Comment")]
        public string Comment { get; set; } // The comment left by the customer

        [BsonElement("Rating")]
        public int Rating { get; set; } // Rating from 1 to 5

    }
}
