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

            // Fetch all products and inventories
            var allProducts = _products.Find(_ => true).ToList();
            var allProductInventories = _productInventories.Find(_ => true).ToList();

            // Update IsActive status based on inventory levels
            foreach (var productInventory in allProductInventories)
            {
                var product = allProducts.FirstOrDefault(p => p.Id == productInventory.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for Inventory ID: {inventoryId}.", productInventory.Id);
                    continue;
                }

                bool isActiveBefore = product.IsActive;
                product.IsActive = productInventory.Quantity > productInventory.ReorderLevel;

                if (product.IsActive != isActiveBefore)
                {
                    _logger.LogInformation("Product ID: {productId} status changed from {oldStatus} to {newStatus}.",
                        product.Id, isActiveBefore, product.IsActive);

                    _products.ReplaceOne(p => p.Id == product.Id, product);

                    // If the status changed from active (true) to inactive (false), send a restock notification
                    if (isActiveBefore && !product.IsActive)
                    {
                        _logger.LogWarning("Product ID: {productId} became inactive due to low inventory. Notifying Vendor ID: {vendorId}.",
                            product.Id, productInventory.VendorId);

                        SendRestockNotification(productInventory.VendorId, product.Name, productInventory.Quantity);
                    }
                }

            }

            // Process orders with unchecked quantities
            var ordersToCheck = _orders.Find(order => !order.IsQuantityChecked).ToList();
            _logger.LogInformation("Processing {orderCount} orders with unchecked quantities.", ordersToCheck.Count);

            foreach (var order in ordersToCheck)
            {
                foreach (var item in order.OrderItems)
                {
                    var productInventory = allProductInventories.FirstOrDefault(pi => pi.ProductId == item.ProductId);
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
                        var product = allProducts.FirstOrDefault(p => p.Id == productInventory.ProductId);
                        if (product != null)
                        {
                            product.IsActive = false;
                            _products.ReplaceOne(p => p.Id == product.Id, product);
                        }

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
