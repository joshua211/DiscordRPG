using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;

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

    Task<Result<IEnumerable<DungeonReadModel>>> GetAllDungeonsAsync(TransactionContext parentContext,
        CancellationToken token = default);

    Task<Result> DeleteDungeonAsync(GuildId guildId, DungeonId dungeonId, TransactionContext parentContext,
        CancellationToken token = default);
}