using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface IActivityRepository
{
    Task<Activity> GetActivityAsync(string id, CancellationToken token);
    Task SaveActivityAsync(Activity requestActivity, CancellationToken cancellationToken);
    Task DeleteActivityAsync(string requestId, CancellationToken cancellationToken);
}