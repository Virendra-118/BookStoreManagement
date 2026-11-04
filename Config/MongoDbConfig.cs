using MongoDB.Driver;

namespace BookStoreManagement.Config
{
    public class MongoDbConfig
    {
        private readonly IMongoDatabase _database;

        public MongoDbConfig(IConfiguration configuration)
        {
            var connectionString =
                configuration["MongoDB:ConnectionString"];

            var databaseName =
                configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);

            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase Database => _database;
    }
}