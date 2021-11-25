using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IActivityService
{
    Task<Result> QueueActivityAsync(string charId, TimeSpan duration, ActivityType type, ActivityData data,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Activity>> GetCharacterActivityAsync(string charId, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<Activity>> GetActivityAsync(string activityId, TransactionContext parentContext = null,
        CancellationToken token = default);
}