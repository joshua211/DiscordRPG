using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IActivityService
{
    Task<Result> QueueActivityAsync(Identity charId, ActivityDuration duration, ActivityType type, ActivityData data,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Activity>> GetCharacterActivityAsync(Identity charId, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<Activity>> GetActivityAsync(Identity activityId, TransactionContext parentContext = null,
        CancellationToken token = default);
}