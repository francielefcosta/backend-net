// Services/MongoDbService.cs
using MongoDB.Driver;
using MyProject.Models;

namespace MyProject.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _products;

        public MongoDbService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _products = _database.GetCollection<Product>("products"); 
        }

        public IMongoCollection<T> GetCollection<T>(string nomeColecao)
        {
            return _database.GetCollection<T>(nomeColecao);
        }

        public async Task CreateProductAsync(Product product)
        {
            await _products.InsertOneAsync(product);
        }
    }
}
