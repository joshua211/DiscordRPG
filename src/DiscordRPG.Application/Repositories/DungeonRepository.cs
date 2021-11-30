using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class DungeonRepository : IRepository<Dungeon>
{
    private readonly IMongoCollection<Dungeon> dungeons;
    private readonly ILogger logger;

    public DungeonRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        dungeons = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Dungeon>(databaseSettings.DungeonCollectionName);
    }

    public async Task SaveAsync(Dungeon entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Saving dungeon {@Dungeon}", entity);
        await dungeons.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Dungeon entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Updating dungeon {@entity}", entity);
        entity.LastModified = DateTime.UtcNow;
        var result = await dungeons.ReplaceOneAsync(d => d.ID == entity.ID, entity,
            cancellationToken: cancellationToken);
        logger.Here().Verbose("Updated {Count} dungeons", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Deleting dungeon with id {Id}", id);
        var result = await dungeons.DeleteOneAsync(d => d.ID == id, cancellationToken);
        logger.Here().Verbose("Deleted {Count} dungeons", result.DeletedCount);
    }

    public async Task<Dungeon> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting dungeon with id {Id}", id);
        var cursor = await dungeons.FindAsync(d => d.ID == id, cancellationToken: cancellationToken);
        var dungeon = await cursor.FirstOrDefaultAsync(cancellationToken);
        logger.Here().Verbose("Found dungeon {@Dungeon}", dungeon);

        return dungeon;
    }

    public async Task<IEnumerable<Dungeon>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting all dungeons");
        var cursor = await dungeons.FindAsync(d => true, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found {Count} dungeons", list.Count);

        return list;
    }

    public async Task<IEnumerable<Dungeon>> FindAsync(Expression<Func<Dungeon, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding dungeons with expression {Expression}", expression);
        var cursor = await dungeons.FindAsync(expression, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found {Count} dungeons", list.Count);

        return list;
    }
}