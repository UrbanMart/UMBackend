using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class OrdersService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrdersService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>("Orders");
        }

        public List<Order> Get() => _orders.Find(order => true).ToList();

        public Order Get(string id) => _orders.Find(order => order.Id == id).FirstOrDefault();

        public Order Create(Order order)
        {
            _orders.InsertOne(order);
            return order;
        }

        public void Update(string id, Order updatedOrder) =>
            _orders.ReplaceOne(order => order.Id == id, updatedOrder);

        public void Delete(string id) =>
            _orders.DeleteOne(order => order.Id == id);
    }
}
