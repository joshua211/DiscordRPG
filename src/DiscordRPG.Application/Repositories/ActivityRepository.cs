using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly IMongoCollection<Activity> activities;

    public ActivityRepository(IActivityDatabaseSettings databaseSettings)
    {
        var client = new MongoClient(databaseSettings.ConnectionString);
        activities = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Activity>(databaseSettings.CollectionName);
    }


    public async Task<Activity> GetActivityAsync(string id, CancellationToken token)
    {
        var result = await activities.FindAsync(a => a.ID == id, cancellationToken: token);

        return await result.FirstOrDefaultAsync(token);
    }

    public async Task SaveActivityAsync(Activity activity, CancellationToken cancellationToken)
    {
        await activities.InsertOneAsync(activity, cancellationToken: cancellationToken);
    }

    public async Task DeleteActivityAsync(string id, CancellationToken cancellationToken)
    {
        await activities.DeleteOneAsync(a => a.ID == id, cancellationToken: cancellationToken);
    }
}