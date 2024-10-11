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
            _logger.LogInformation("Starting inventory check process at {time}", DateTime.Now);

            // Fetch all products and inventories
            var allProducts = _products.Find(product => true).ToList();
            var allProductInventories = _productInventories.Find(pi => true).ToList();

            // Update IsActive status based on inventory levels
            foreach (var productInventory in allProductInventories)
            {
                var product = allProducts.FirstOrDefault(p => p.Id == productInventory.ProductId);
                if (product != null)
                {
                    bool isActiveBefore = product.IsActive;
                    product.IsActive = productInventory.Quantity > productInventory.ReorderLevel;

                    // Log the change in the IsActive status
                    if (product.IsActive != isActiveBefore)
                    {
                        _logger.LogInformation("Product ID: {productId} IsActive status changed from {oldStatus} to {newStatus}",
                            product.Id, isActiveBefore, product.IsActive);
                    }

                    // Update the product status in the database
                    _products.ReplaceOne(p => p.Id == product.Id, product);
                    _logger.LogInformation("Updated Product ID: {productId} in database with IsActive = {isActive}.", product.Id, product.IsActive);
                }
                else
                {
                    _logger.LogWarning("Product ID: {productId} not found for inventory ID: {inventoryId}.",
                        productInventory.ProductId, productInventory.Id);
                }
            }

            // Get all orders where IsQuantityChecked is false
            var ordersToCheck = _orders.Find(order => !order.IsQuantityChecked).ToList();

            _logger.LogInformation("Found {orderCount} orders with unchecked quantities.", ordersToCheck.Count);

            foreach (var order in ordersToCheck)
            {
                _logger.LogInformation("Processing Order ID: {orderId}, Customer: {customerName}", order.Id, order.CustomerName);

                foreach (var item in order.OrderItems)
                {
                    var productInventory = allProductInventories.FirstOrDefault(pi => pi.ProductId == item.ProductId);

                    if (productInventory != null)
                    {
                        _logger.LogInformation("Checking Product ID: {productId}, Current Quantity: {quantity}, Ordered Quantity: {orderedQuantity}",
                            item.ProductId, productInventory.Quantity, item.Quantity);

                        // Deduct quantity from ProductInventory
                        productInventory.Quantity -= item.Quantity;

                        // Log the updated quantity
                        _logger.LogInformation("Deducted {quantity} from Product ID: {productId}. New Quantity: {newQuantity}.",
                            item.Quantity, item.ProductId, productInventory.Quantity);

                        // Update the inventory in the database
                        _productInventories.ReplaceOne(pi => pi.ProductId == productInventory.ProductId, productInventory);
                        _logger.LogInformation("Updated Product Inventory for Product ID: {productId} in database.", item.ProductId);

                        // If the quantity falls below ReorderLevel, send a restock notification
                        if (productInventory.Quantity <= productInventory.ReorderLevel)
                        {
                            _logger.LogWarning("Product ID: {productId} has low inventory ({quantity}). Notifying Vendor ID: {vendorId}",
                                item.ProductId, productInventory.Quantity, productInventory.VendorId);

                            // Include the current quantity in the restock notification message
                            SendRestockNotification(productInventory.VendorId, productInventory.Name, productInventory.Quantity);
                        }
                    }
                    else
                    {
                        _logger.LogError("Product ID: {productId} not found in inventory for Order ID: {orderId}.", item.ProductId, order.Id);
                    }
                }

                // Mark order as Quantity Checked
                order.IsQuantityChecked = true;
                _orders.ReplaceOne(o => o.Id == order.Id, order);
                _logger.LogInformation("Order ID: {orderId} marked as Quantity Checked.", order.Id);
            }

            _logger.LogInformation("Inventory check process completed at {time}", DateTime.Now);
        }

        private void SendRestockNotification(string vendorId, string productName, int currentQuantity)
        {
            var notification = new Notification
            {
                UserId = vendorId,
                Message = $"Please restock {productName}. Current inventory is {currentQuantity}.",
                Type = "Restock",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _notificationsService.Create(notification);

            _logger.LogInformation("Restock notification sent to Vendor ID: {vendorId} for Product: {productName}, Current Quantity: {currentQuantity}",
                vendorId, productName, currentQuantity);
        }
    }
}
