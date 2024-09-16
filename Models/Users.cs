using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; } // Administrator, Vendor, CSR

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        // Optional: You can add more fields based on the requirements
        [BsonElement("Name")]
        public string Name { get; set; }

       
    }
}
