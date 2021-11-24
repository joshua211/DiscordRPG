using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface IDungeonRepository
{
    Task SaveDungeonAsync(Dungeon dungeon, CancellationToken cancellationToken);
}