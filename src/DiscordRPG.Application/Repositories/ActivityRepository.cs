using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class ActivityRepository : IRepository<Activity>
{
    private readonly IMongoCollection<Activity> activities;
    private readonly ILogger logger;

    public ActivityRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        activities = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Activity>(databaseSettings.ActivityCollectionName);
    }


    public async Task SaveAsync(Activity entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving character {@Activity}", entity);
        await activities.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Activity entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Updating Character {@Activity}", entity);
        var result = await activities.ReplaceOneAsync(e => e.ID == entity.ID, entity,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} Activities", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting Activity {Act}", id);
        var result = await activities.DeleteOneAsync(c => c.ID == id, cancellationToken);
        logger.Verbose("Deleted {Count} Activities", result.DeletedCount);
    }

    public async Task<Activity> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Getting Activity {Id}", id);
        var result = await activities.FindAsync(e => e.ID == id, cancellationToken: cancellationToken);

        var entity = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        logger.Verbose("Found Activity: {@Character}", entity);
        return entity;
    }

    public async Task<IEnumerable<Activity>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Verbose("Getting all Activities");

        var result = await activities.FindAsync(c => true, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Activity>> FindAsync(Expression<Func<Activity, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding Activities with expression {Expression}", expression);
        var cursor = await activities.FindAsync(expression, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }
}