using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface IGuildRepository
{
    Task<Guild> GetGuildAsync(ulong guildId, CancellationToken token);
    Task SaveGuildAsync(Guild guild, CancellationToken cancellationToken);
}