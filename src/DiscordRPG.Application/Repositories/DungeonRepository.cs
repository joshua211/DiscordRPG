using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class DungeonRepository : IRepository<Dungeon>
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

    public async Task SaveAsync(Dungeon entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving dungeon {@Dungeon}", entity);
        await dungeons.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Dungeon entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Updating dungeon {@entity}", entity);
        var result = await dungeons.ReplaceOneAsync(d => d.ID == entity.ID, entity,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} dungeons", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting dungeon with id {Id}", id);
        await dungeons.DeleteOneAsync(d => d.ID == id, cancellationToken);
    }

    public async Task<Dungeon> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Getting dungeon with id {Id}", id);
        var cursor = await dungeons.FindAsync(d => d.ID == id, cancellationToken: cancellationToken);

        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Dungeon>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Verbose("Getting all dungeons");
        var cursor = await dungeons.FindAsync(d => true, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Dungeon>> FindAsync(Expression<Func<Dungeon, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding dungeons with expression {Expression}", expression);
        var cursor = await dungeons.FindAsync(expression, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }
}