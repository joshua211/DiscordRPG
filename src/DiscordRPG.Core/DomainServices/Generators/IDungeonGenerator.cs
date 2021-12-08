using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Generators;

public interface IDungeonGenerator
{
    Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel, ActivityDuration duration);
}