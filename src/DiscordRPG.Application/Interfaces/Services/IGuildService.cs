using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IGuildService
{
    Task<Result<Guild>> GetGuildAsync(Identity identity, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Guild>> GetGuildWithDiscordIdAsync(DiscordId serverId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Guild>> CreateGuildAsync(DiscordId serverId, string guildName, DiscordId guildHallId,
        DiscordId dungeonHallId,
        TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> DeleteGuildAsync(DiscordId serverId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);
}