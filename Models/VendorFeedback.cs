/*
 * File: VendorFeedback.cs
 * Description: This file defines the VendorFeedback model for the UrbanMart application,
 *              which represents the feedback that customers provide for vendors after a purchase.
 *              It includes properties such as vendor ID, customer ID, rating, and comment.
 * Author: Darshi Buddhini
 * Date: 22/09/24
 * 
 * The VendorFeedback model is stored in the MongoDB database, representing feedback given
 * by customers regarding their experience with vendors. The rating is an integer value between 1 and 5,
 * and the comment is a text string provided by the customer. This feedback is used to calculate the 
 * average ranking of vendors in the system.
 */
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
