/*
 * File: User.cs
 * Description: This file defines the User model for the UrbanMart application, 
 *              which represents the user entity in the MongoDB database.
 * Author: Darshi Buddhini
 * Date: 16/09/24
 * 
 * The User model includes fields for storing essential user information like 
 * email, password, role, and account status, with necessary MongoDB BSON attributes.
 */

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    /// The User class defines the user entity with relevant properties for the 
    /// UrbanMart system. This model interacts with MongoDB collections.
    public class User
    {
        // The unique identifier for each user (MongoDB ObjectId).
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Stores the user's email address.
        [BsonElement("Email")]
        public string Email { get; set; }

        // Stores the user's password in encrypted form.
        [BsonElement("Password")]
        public string Password { get; set; }

        // Represents the user's role in the system, such as Administrator, Vendor, or CSR.
        [BsonElement("Role")]
        public string Role { get; set; } // Administrator, Vendor, CSR

        // Indicates whether the user's account is active.
        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        // Stores the name of the user.
        [BsonElement("Name")]
        public string Name { get; set; }

        
    }
}
