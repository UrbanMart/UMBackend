using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class ProductsService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductsService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _products = database.GetCollection<Product>("Products");
        }

        // Get all products, including filtering by active status and optionally filtering by vendor ID
        public List<Product> Get(bool includeInactive = false, string vendorId = null)
        {
            var filterBuilder = Builders<Product>.Filter;
            var filter = includeInactive ? filterBuilder.Empty : filterBuilder.Eq(p => p.IsActive, true);

            if (!string.IsNullOrEmpty(vendorId))
            {
                var vendorFilter = filterBuilder.Eq(p => p.VendorId, vendorId);
                filter = filter & vendorFilter;
            }

            return _products.Find(filter).ToList();
        }

        // Get a product by ID
        public Product Get(string id) => 
            _products.Find(product => product.Id == id).FirstOrDefault();

        // Create a new product
        public Product Create(Product product)
        {
            _products.InsertOne(product);
            return product;
        }

        // Update a product
        public void Update(string id, Product updatedProduct) =>
            _products.ReplaceOne(product => product.Id == id, updatedProduct);

        // Delete a product
        public void Delete(string id) =>
            _products.DeleteOne(product => product.Id == id);

        // Activate a product
        public void ActivateProduct(string id)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            _products.UpdateOne(product => product.Id == id, update);
        }

        // Deactivate a product
        public void DeactivateProduct(string id)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            _products.UpdateOne(product => product.Id == id, update);
        }
    }
}
