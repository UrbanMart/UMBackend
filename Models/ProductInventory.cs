using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    public class ProductInventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("ReorderLevel")]
        public int ReorderLevel { get; set; }

        [BsonElement("VendorId")]
        public string VendorId { get; set; }

      
    }
}
