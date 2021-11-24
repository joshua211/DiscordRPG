using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class ActivityRepository : IActivityRepository
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


    public async Task<Activity> GetActivityAsync(string id, CancellationToken token)
    {
        logger.Verbose("Getting Activity {ID} ", id);
        var result = await activities.FindAsync(a => a.ID == id, cancellationToken: token);

        var activity = await result.FirstOrDefaultAsync(token);

        logger.Verbose("Found activity: {@Activity}", activity);
        return activity;
    }

    public async Task SaveActivityAsync(Activity activity, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving activity {@Activity}", activity);
        await activities.InsertOneAsync(activity, cancellationToken: cancellationToken);
    }

    public async Task DeleteActivityAsync(string id, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting activity with id {Id}", id);
        var result = await activities.DeleteOneAsync(a => a.ID == id, cancellationToken: cancellationToken);
        logger.Verbose("Deleted {Count} activities", result.DeletedCount);
    }

    public async Task<IEnumerable<Activity>> FindAsync(Expression<Func<Activity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var result = await activities.FindAsync(predicate, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }
}