using urbanmart.Models;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace urbanmart.Services
{
    public class InventoryCheckService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<ProductInventory> _productInventories;
        private readonly NotificationsService _notificationsService;
        private readonly ILogger<InventoryCheckService> _logger;

        public InventoryCheckService(IDatabaseSettings settings, NotificationsService notificationsService, ILogger<InventoryCheckService> logger)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _products = database.GetCollection<Product>("Products");
            _orders = database.GetCollection<Order>("Orders");
            _productInventories = database.GetCollection<ProductInventory>("ProductInventory");
            _notificationsService = notificationsService;
            _logger = logger;
        }

        public void CheckAndProcessOrders()
        {
            _logger.LogInformation("Inventory check process started at {time}", DateTime.Now);
            
            // Process orders with unchecked quantities
            var ordersToCheck = _orders.Find(order => !order.IsQuantityChecked).ToList();
            _logger.LogInformation("Processing {orderCount} orders with unchecked quantities.", ordersToCheck.Count);

            foreach (var order in ordersToCheck)
            {
                foreach (var item in order.OrderItems)
                {
                    var productInventory = _productInventories.Find(pi => pi.VendorId == item.VendorId && pi.ProductId == item.ProductId).FirstOrDefault();
                    if (productInventory == null)
                    {
                        _logger.LogError("Product ID: {productId} not found in inventory for Order ID: {orderId}.", item.ProductId, order.Id);
                        continue;
                    }

                    // Deduct quantity and update inventory
                    productInventory.Quantity -= item.Quantity;
                    _productInventories.ReplaceOne(pi => pi.ProductId == productInventory.ProductId, productInventory);

                    if (productInventory.Quantity <= productInventory.ReorderLevel)
                    {
                       _logger.LogWarning("Product ID: {productId} has low inventory ({quantity}). Notifying Vendor ID: {vendorId}",
                        item.ProductId, productInventory.Quantity, productInventory.VendorId);
                        // Send restock notification
                        SendRestockNotification(productInventory.VendorId, productInventory.Name, productInventory.Quantity);
                    }
                }

                // Mark order as Quantity Checked
                order.IsQuantityChecked = true;
                _orders.ReplaceOne(o => o.Id == order.Id, order);
            }

            _logger.LogInformation("Inventory check process completed at {time}", DateTime.Now);
        }

        private void SendRestockNotification(string vendorId, string productName, int currentQuantity)
        {
            var notification = new Notification
            {
                UserId = vendorId,
                Message = $"Please restock {productName}. Current inventory: {currentQuantity}.",
                Type = "Restock",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _notificationsService.Create(notification);
            _logger.LogInformation("Restock notification sent to Vendor ID: {vendorId} for Product: {productName}.", vendorId, productName);
        }
    }
}
