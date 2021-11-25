using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IGuildService
{
    Task<Result<Guild>> GetGuildAsync(ulong guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Guild>> CreateGuildAsync(ulong guildId, string guildName, ulong guildHallId, ulong dungeonHallId,
        TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> DeleteGuildAsync(ulong id, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);
}