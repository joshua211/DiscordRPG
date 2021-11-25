using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IDungeonService
{
    Task<Result<Dungeon>> CreateDungeonAsync(ulong guildId, ulong threadId, uint charLevel,
        TransactionContext parentContext = null,
        CancellationToken token = default);
}