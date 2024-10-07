/*
 * File: Product.cs
 * Description: This file defines the Product model for the UrbanMart application.
 *              It represents individual product details including name, price, category,
 *              and an image URL. The model is stored in a MongoDB collection.
 * Author: Imesh Vitharana
 * Date: 15/09/24
 * 
 * The Product class is used to manage the product catalog for UrbanMart.
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = false; 

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }
    }
}
