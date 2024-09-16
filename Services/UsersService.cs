using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _users;

        public UsersService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        // Get all users
        public List<User> Get() => _users.Find(user => true).ToList();

        // Get a user by Id
        public User Get(string id) => _users.Find(user => user.Id == id).FirstOrDefault();

        // Create a new user
        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        // Update an existing user
        public void Update(string id, User updatedUser) =>
            _users.ReplaceOne(user => user.Id == id, updatedUser);

        // Delete a user by Id
        public void Delete(string id) =>
            _users.DeleteOne(user => user.Id == id);
    }
}
