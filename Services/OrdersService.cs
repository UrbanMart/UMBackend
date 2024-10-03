/*
 * File: OrdersService.cs
 * Description: This file defines the OrdersService class for the UrbanMart application, 
 *              which provides methods for managing order-related operations in the 
 *              MongoDB database, such as creating orders with product details, 
 *              retrieving, updating, and deleting orders, as well as vendor-specific operations.
 * Author: Imesh Vitharana
 * Date: 15/09/24
 * 
 * The OrdersService class interacts with the MongoDB collections for orders and products 
 * and provides functionality for order management, including calculating product details 
 * within an order, handling vendor-specific operations, and managing order status updates.
 */

using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class OrdersService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Product> _products; // Collection for Products

        public OrdersService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>("Orders");
            _products = database.GetCollection<Product>("Products"); // Initialize products collection
        }

        // Retrieve all orders
        public List<Order> Get() => _orders.Find(order => true).ToList();

        // Retrieve a single order by ID
        public Order Get(string id) => _orders.Find(order => order.Id == id).FirstOrDefault();

        // Create a new order with product details
        public Order Create(Order order)
        {
            // Populate product details in OrderItems
            foreach (var orderItem in order.OrderItems)
            {
                // Retrieve product details from the Products collection using ProductId
                var product = _products.Find(p => p.Id == orderItem.ProductId).FirstOrDefault();
                if (product != null)
                {
                    // Set product details in the OrderItem
                    orderItem.ProductName = product.Name; 
                    orderItem.UnitPrice = product.Price; 
                    orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice; 
                }
                else
                {
                    // Handle case where the product does not exist (optional logging or handling)
                    orderItem.ProductName = "Unknown Product"; // Set a default name or handle it as needed
                    orderItem.UnitPrice = 0; // Set to zero if product not found
                    orderItem.TotalPrice = 0; // Set to zero if product not found
                }
            }

            _orders.InsertOne(order); // Saves the order to the database
            return order; // Returns the created order
        }

        // Update an existing order
        public void Update(string id, Order updatedOrder) =>
            _orders.ReplaceOne(order => order.Id == id, updatedOrder);

        // Delete an order by ID
        public void Delete(string id) =>
            _orders.DeleteOne(order => order.Id == id);

        // Get order items for a specific vendor
        public List<OrderItem> GetVendorOrderItems(string vendorId)
        {
            // Find orders containing items for the specified vendor
            var orders = _orders.Find(order => order.OrderItems.Any(item => item.VendorId == vendorId)).ToList();
            List<OrderItem> vendorOrderItems = new List<OrderItem>();

            foreach (var order in orders)
            {
                var items = order.OrderItems.Where(item => item.VendorId == vendorId).ToList();
                vendorOrderItems.AddRange(items);
            }

            return vendorOrderItems; // Return the list of order items for the vendor
        }

        // Method to mark an order item as delivered and update the overall order status
        public void MarkItemAsDelivered(string orderId, string productId, string vendorId)
        {
            var order = Get(orderId);
            if (order != null)
            {
                // Find the specific item in the order to mark as delivered
                var item = order.OrderItems.Find(i => i.ProductId == productId && i.VendorId == vendorId);
                if (item != null)
                {
                    item.Status = "Delivered";
                    Update(orderId, order); // Update the order with the new status
                    CheckOrderStatus(order); // Check and update the overall order status
                }
            }
        }

        // Check if all items are delivered and update the order status accordingly
        private void CheckOrderStatus(Order order)
        {
            // If all items are delivered, set the order status to "Delivered"
            if (order.OrderItems.All(item => item.Status == "Delivered"))
            {
                order.Status = "Delivered";
            }
            // If any item is still in "Created" status, set the order status to "Partially Delivered"
            else if (order.OrderItems.Any(item => item.Status == "Created"))
            {
                order.Status = "Partially Delivered";
            }
            Update(order.Id, order); // Save the updated order status in the database
        }
    }
}
