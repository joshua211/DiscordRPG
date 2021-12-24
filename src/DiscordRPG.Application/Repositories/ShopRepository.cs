using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class ShopRepository : IRepository<Shop>
{
    private readonly ILogger logger;
    private readonly IMongoCollection<Shop> shops;

    public ShopRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        shops = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Shop>(databaseSettings.ShopCollectionName);
    }

    public async Task SaveAsync(Shop entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Saving Shop {@Shop}", entity);
        await shops.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Shop entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Updating Shop {@entity}", entity);
        entity.LastModified = DateTime.UtcNow;
        var result = await shops.ReplaceOneAsync(d => d.ID.Value == entity.ID.Value, entity,
            cancellationToken: cancellationToken);
        logger.Here().Verbose("Updated {Count} Shops", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Deleting Shop with id {Id}", id);
        var result = await shops.DeleteOneAsync(d => d.ID.Value == id.Value, cancellationToken);
        logger.Here().Verbose("Deleted {Count} Shops", result.DeletedCount);
    }

    public async Task<Shop> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting Shop with id {Id}", id);
        var cursor = await shops.FindAsync(d => d.ID.Value == id.Value, cancellationToken: cancellationToken);
        var shop = await cursor.FirstOrDefaultAsync(cancellationToken);
        logger.Here().Verbose("Found Shop {@Shop}", shop);

        return shop;
    }

    public async Task<IEnumerable<Shop>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting all Shops");
        var cursor = await shops.FindAsync(d => true, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found {Count} Shops", list.Count);

        return list;
    }

    public async Task<IEnumerable<Shop>> FindAsync(Expression<Func<Shop, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding dungeons with expression {Expression}", expression);
        var cursor = await shops.FindAsync(expression, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found {Count} Shops", list.Count);

        return list;
    }
}