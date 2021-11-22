using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IActivityService
{
    Task<Result> QueueActivityAsync(ulong userId, DateTime start, TimeSpan duration, ActivityType type,
        CancellationToken cancellationToken = default);
}