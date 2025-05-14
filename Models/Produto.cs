// Models/Product.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyProject.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("img")]
        public string Img { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

    }
}
