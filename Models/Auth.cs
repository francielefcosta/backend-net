
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyProject.Models
{
    public class Login
    {
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class Register
    {
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
    }
}