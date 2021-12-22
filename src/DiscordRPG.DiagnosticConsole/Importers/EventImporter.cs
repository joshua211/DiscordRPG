using DiscordRPG.DiagnosticConsole.Settings;
using MongoDB.Driver;

namespace DiscordRPG.DiagnosticConsole.Importers;

public class EventImporter : IEventImporter
{
    private readonly IMongoCollection<object> collection;

    public EventImporter(IMongoClient mongoClient, IDatabaseSettings settings)
    {
        collection = mongoClient.GetDatabase(settings.DatabaseName)
            .GetCollection<object>(settings.EventCollectionName);
    }

    public async Task<IEnumerable<object>> GetEventsAsync()
    {
        var result = await collection.FindAsync(d => true);

        return result.ToList();
    }
}

public interface IEventImporter
{
    Task<IEnumerable<object>> GetEventsAsync();
}