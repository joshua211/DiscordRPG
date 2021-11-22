using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface IGuildRepository
{
    Task<Guild> GetGuildAsync(ulong guildId, CancellationToken token);
    Task SaveGuildAsync(Guild guild, CancellationToken cancellationToken);

    Task UpdateGuildAsync(Guild guild, CancellationToken cancellationToken);

    Task DeleteGuildAsync(ulong guildId, CancellationToken cancellationToken);
}