using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class ProductInventoryService
    {
        private readonly IMongoCollection<ProductInventory> _productInventory;

        public ProductInventoryService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _productInventory = database.GetCollection<ProductInventory>("ProductInventory");
        }

        // Get all products
        public List<ProductInventory> Get() => _productInventory.Find(product => true).ToList();

        // Get a product by ID
        public ProductInventory Get(string id) => _productInventory.Find(product => product.Id == id).FirstOrDefault();

        // Create a new product with inventory
        public ProductInventory Create(ProductInventory product)
        {
            _productInventory.InsertOne(product);
            return product;
        }

        // Update product and inventory
        public void Update(string id, ProductInventory updatedProduct) =>
            _productInventory.ReplaceOne(product => product.Id == id, updatedProduct);

        // Update inventory levels only
        public void UpdateInventory(string id, int newQuantity)
        {
            var product = Get(id);
            if (product != null)
            {
                product.Quantity = newQuantity;
                _productInventory.ReplaceOne(p => p.Id == id, product);
            }
        }

        // Get products with low stock
        public List<ProductInventory> GetLowStockItems(int reorderLevel) =>
            _productInventory.Find(product => product.Quantity <= reorderLevel).ToList();

        // Delete a product
        public void Delete(string id) =>
            _productInventory.DeleteOne(product => product.Id == id);
    }
}
