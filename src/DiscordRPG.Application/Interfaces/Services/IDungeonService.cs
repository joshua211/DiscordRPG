using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IDungeonService
{
    Task<Result<Dungeon>> CreateDungeonAsync(DiscordId serverId, DiscordId threadId, uint charLevel, Rarity rarity,
        TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<Dungeon>> GetDungeonFromChannelIdAsync(DiscordId channelId, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> CalculateDungeonAdventureResultAsync(Identity chadId, DiscordId threadId,
        TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> DecreaseExplorationsAsync(Dungeon dialogDungeon, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<IEnumerable<Dungeon>>> GetAllDungeonsAsync(TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> DeleteDungeonAsync(Dungeon dungeon, TransactionContext parentContext = null,
        CancellationToken token = default);
}