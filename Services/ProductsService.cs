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

        // Get all products
        public List<Product> Get() => _products.Find(product => true).ToList();

        // Get a product by ID
        public Product Get(string id) => _products.Find(product => product.Id == id).FirstOrDefault();

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
    }
}
