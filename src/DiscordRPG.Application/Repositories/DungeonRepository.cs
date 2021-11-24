using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class DungeonRepository : IDungeonRepository
{
    private readonly IMongoCollection<Dungeon> dungeons;
    private readonly ILogger logger;

    public DungeonRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        dungeons = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Dungeon>(databaseSettings.DungeonCollectionName);
    }

    public async Task SaveDungeonAsync(Dungeon dungeon, CancellationToken cancellationToken)
    {
        await dungeons.InsertOneAsync(dungeon, null, cancellationToken);
    }
}