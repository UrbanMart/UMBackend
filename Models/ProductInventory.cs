/*
 * File: ProductInventory.cs
 * Description: This file defines the ProductInventory model for the UrbanMart application. 
 *              It represents the inventory for products, including properties like 
 *              name, quantity, reorder level, and vendor information.
 * Author: Darshi Buddhini
 * Date: 16/09/24
 * 
 * The ProductInventory model is used to manage and track inventory levels for 
 * products in the MongoDB database.
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    public class ProductInventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        // The quantity of the product available in the inventory.
        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        // The minimum quantity at which reordering should occur.
        [BsonElement("ReorderLevel")]
        public int ReorderLevel { get; set; }

        [BsonElement("VendorId")]
        public string VendorId { get; set; }
    }
}
