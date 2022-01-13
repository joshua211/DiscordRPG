using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IGuildService
{
    Task<Result<GuildReadModel>> GetGuildAsync(GuildId identity, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> CreateGuildAsync(string serverId, string guildName, string guildHallId,
        string dungeonHallId, string innChannel,
        TransactionContext context, CancellationToken token = default);

    Task<Result> DeleteGuildAsync(GuildId id, TransactionContext parentContext,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<GuildReadModel>>> GetAllGuildsAsync(TransactionContext parentContext,
        CancellationToken cancellationToken = default);
}