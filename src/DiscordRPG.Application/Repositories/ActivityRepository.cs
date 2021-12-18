using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class ActivityRepository : IRepository<Activity>
{
    private readonly IMongoCollection<Activity> activities;
    private readonly ILogger logger;

    public ActivityRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        activities = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Activity>(databaseSettings.ActivityCollectionName);
    }


    public async Task SaveAsync(Activity entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Saving character {@Activity}", entity);
        await activities.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Activity entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Updating Character {@Activity}", entity);
        entity.LastModified = DateTime.UtcNow;
        var result = await activities.ReplaceOneAsync(e => e.ID.Value == entity.ID.Value, entity,
            cancellationToken: cancellationToken);
        logger.Here().Verbose("Updated {Count} Activities", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Deleting Activity {Act}", id);
        var result = await activities.DeleteOneAsync(c => c.ID.Value == id.Value, cancellationToken);
        logger.Here().Verbose("Deleted {Count} Activities", result.DeletedCount);
    }

    public async Task<Activity> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting Activity {Id}", id);
        var result = await activities.FindAsync(e => e.ID.Value == id.Value, cancellationToken: cancellationToken);

        var entity = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        logger.Verbose("Found Activity: {@Character}", entity);
        return entity;
    }

    public async Task<IEnumerable<Activity>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting all Activities");

        var result = await activities.FindAsync(c => true, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Activity>> FindAsync(Expression<Func<Activity, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Finding Activities with expression {Expression}", expression);
        var cursor = await activities.FindAsync(expression, cancellationToken: cancellationToken);
        var result = await cursor.ToListAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found {Count} activities", result.Count);

        return result;
    }
}