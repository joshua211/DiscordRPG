using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Activity.ValueObjects;
using DiscordRPG.Domain.Entities.Character;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IActivityService
{
    Task<Result> QueueActivityAsync(GuildId id, CharacterId characterId, ActivityDuration duration, ActivityType type,
        ActivityData data,
        TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result<ActivityReadModel>> GetCharacterActivityAsync(CharacterId characterId, TransactionContext context,
        CancellationToken token = default);

    Task<Result<ActivityReadModel>> GetActivityAsync(ActivityId id, TransactionContext context,
        CancellationToken token = default);

    Task<Result> StopActivityAsync(GuildId guildId, ActivityId activityId, TransactionContext context,
        CancellationToken token = default);

    Task<Result<IEnumerable<ActivityReadModel>>> GetAllActivitiesAsync(TransactionContext context,
        CancellationToken token = default);

    Task<Result> CompleteActivityAsync(GuildId guildId, ActivityId activityId, TransactionContext context);
}