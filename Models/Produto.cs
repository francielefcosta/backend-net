// Models/Product.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyProject.Models
{
    [BsonIgnoreExtraElements]
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

    [BsonIgnoreExtraElements]
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = "0";
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public IFormFile? Img { get; set; }
        public string AdminPassword { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class ProductDelete
    {
        public string AdminPassword { get; set; } = string.Empty;
    }
}
