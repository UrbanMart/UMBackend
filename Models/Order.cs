using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace urbanmart.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("CustomerName")]
        public string CustomerName { get; set; }

        [BsonElement("OrderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("OrderItems")]
        public List<OrderItem> OrderItems { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }
    }

    public class OrderItem
    {
        [BsonElement("ProductId")]
        public string ProductId { get; set; }

        [BsonElement("ProductName")]
        public string ProductName { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
