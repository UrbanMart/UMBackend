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

        public List<Product> Get() => _products.Find(product => true).ToList();

        public Product Get(string id) => _products.Find(product => product.Id == id).FirstOrDefault();

        public Product Create(Product product)
        {
            _products.InsertOne(product);
            return product;
        }

        public void Update(string id, Product updatedProduct) =>
            _products.ReplaceOne(product => product.Id == id, updatedProduct);

        public void Delete(string id) =>
            _products.DeleteOne(product => product.Id == id);
    }
}
