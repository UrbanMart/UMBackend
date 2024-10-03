/*
 * File: UsersService.cs
 * Description: This file defines the UsersService class for the UrbanMart application, 
 *              which provides methods for managing user-related operations in the 
 *              MongoDB database, such as creating, updating, retrieving, and deleting users.
 * Author: Darshi Buddhini
 * Date: 16/09/24
 * 
 * The UsersService class interacts with the MongoDB collection for user entities 
 * and provides functionality for authentication, user management, and CRUD operations.
 */
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

        // Retrieve users by their role (CSR)
        public List<User> GetUsersByRole(string role) => _users.Find(user => user.Role == role).ToList();

        // Create a new user
        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        // Update an existing user (when activating an account)
        public void Update(string id, User updatedUser) =>
            _users.ReplaceOne(user => user.Id == id, updatedUser);

        // Delete a user by Id
        public void Delete(string id) =>
            _users.DeleteOne(user => user.Id == id);

        // Login: Check if email and password match
        public User Login(string email, string password)
        {
            // Find the user by email
            var user = _users.Find(u => u.Email == email).FirstOrDefault();
            
            // Check if user exists and the password matches
            if (user != null && user.Password == password)
            {
                return user; // User is authenticated
            }
            
            return null; // Authentication failed
        }
    }
}
