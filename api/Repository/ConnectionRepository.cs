using api.models.db;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api.repository;

public class ConnectionRepository(IOptions<Dictionary<string, DatabaseSettings>> settings)
{
    public Dictionary<string, DatabaseSettings> _settings { get; set; } = settings.Value;

    public IMongoCollection<T> GetCollection<T>(string settingName)
    {
        var dbSettings = _settings[settingName];
        var mongoClient = new MongoClient(dbSettings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);
        return mongoDatabase.GetCollection<T>(dbSettings.CollectionName);
    }
}
