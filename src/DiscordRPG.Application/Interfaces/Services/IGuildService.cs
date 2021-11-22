using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IGuildService
{
    Task<Result<Guild>> GetGuildAsync(ulong guildId, CancellationToken cancellationToken = default);

    Task<Result<Guild>> CreateGuildAsync(ulong guildId, string guildName, ulong guildHallId, ulong dungeonHallId,
        CancellationToken token = default);

    Task<Result> DeleteGuildAsync(ulong id, CancellationToken cancellationToken = default);
}