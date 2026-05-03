using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence;

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoOptions> options)
    {
        var settings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        var client = new MongoClient(settings);
        _database = client.GetDatabase(options.Value.Database);
    }

    public IMongoCollection<User> Users          => _database.GetCollection<User>("users");
    public IMongoCollection<Order> Orders        => _database.GetCollection<Order>("orders");
    public IMongoCollection<Delivery> Deliveries => _database.GetCollection<Delivery>("deliveries");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
}
