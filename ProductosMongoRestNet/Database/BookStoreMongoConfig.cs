using MongoDB.Bson;
using MongoDB.Driver;

namespace ProductosMongoRestNet.Database;

public class BookStoreMongoConfig(ILogger logger)
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BooksCollectionName { get; set; } = null!;


    public void TryConnection()
    {
        logger.LogInformation("Trying to connect to MongoDB");
        var settings = MongoClientSettings.FromConnectionString(ConnectionString);
        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        // Send a ping to confirm a successful connection
        try
        {
            client.GetDatabase("DatabaseName").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            logger.LogInformation("🟢 You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🔴 Error connecting to MongoDB");
            Environment.Exit(1);
        }
    }
}