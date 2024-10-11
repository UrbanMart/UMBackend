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
        public string CustomerName { get; set; } // Name of the customer who placed the order

        [BsonElement("OrderDate")]
        public DateTime OrderDate { get; set; } // Date when the order was placed

        [BsonElement("TotalAmount")]
        public decimal TotalAmount { get; set; } // Total amount for the order

        [BsonElement("OrderItems")]
        public List<OrderItem> OrderItems { get; set; } // List of items in the order

        [BsonElement("Status")]
        public string Status { get; set; } // Status of the order (e.g., "Processing", "Partially Delivered", "Delivered", "Cancelled")
        
        [BsonElement("IsQuantityChecked")]
        public bool IsQuantityChecked { get; set; } = false; // Indicates if the quantity of the order has been checked
    }

    public class OrderItem
    {
        [BsonElement("ProductId")]
        public string ProductId { get; set; } // ID of the product

        [BsonElement("ProductName")]
        public string ProductName { get; set; } // Name of the product

        [BsonElement("VendorId")]
        public string VendorId { get; set; } // ID of the vendor who added the product

        [BsonElement("Quantity")]
        public int Quantity { get; set; } // Quantity of the product ordered

        [BsonElement("UnitPrice")]
        public decimal UnitPrice { get; set; } // Unit price of the product

        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; } // Total price for this order item (Quantity * UnitPrice)

        [BsonElement("Status")]
        public string Status { get; set; } // Status of the item (e.g., "Created", "Delivered")
    }
}