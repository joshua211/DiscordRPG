using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Dungeon;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IDungeonService
{
    Task<Result> CreateDungeonAsync(GuildId guildId, CharacterId character, uint charLevel, int charLuck,
        ActivityDuration duration,
        TransactionContext context, CancellationToken token = default);

    Task<Result<DungeonReadModel>> GetDungeonAsync(DungeonId dungeonId, TransactionContext parentContext,
        CancellationToken token = default);

    Task<Result> CalculateDungeonAdventureResultAsync(GuildId guildId, DungeonId dungeonId, CharacterId characterId,
        ActivityDuration activityDuration,
        TransactionContext parentContext,
        CancellationToken token = default);

    Task<Result> DecreaseExplorationsAsync(GuildId guildId, DungeonId dungeonId, TransactionContext parentContext,
        CancellationToken token = default);

    Task<Result<IEnumerable<DungeonReadModel>>> GetAllDungeonsAsync(GuildId guildId, TransactionContext context,
        CancellationToken token = default);

    Task<Result> DeleteDungeonAsync(GuildId guildId, DungeonId dungeonId, TransactionContext parentContext,
        CancellationToken token = default);
}