using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IActivityService
{
    Task<Result> QueueActivityAsync(string charId, TimeSpan duration, ActivityType type, ActivityData data,
        CancellationToken cancellationToken = default);

    Task<Result<Activity>> GetCharacterActivityAsync(string charId, CancellationToken token);
}