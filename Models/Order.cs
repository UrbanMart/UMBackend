/*
 * File: Order.cs
 * Description: This file defines the Order and OrderItem models for the UrbanMart application.
 *              The Order model represents customer orders, including details such as customer ID, 
 *              order items, total amount, order date, and order status. The OrderItem model represents 
 *              individual items in an order.
 * Author: Imesh Vitharana
 * Date: 15/09/24
 * These models are used to manage and store order data in a MongoDB collection.
 */
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

        [BsonElement("CustomerId")]
        public string CustomerId { get; set; } 

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
