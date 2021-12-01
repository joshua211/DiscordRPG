using DiscordRPG.Application.Interfaces.Generators;

namespace DiscordRPG.Application.Generators;

public class DungeonGenerator : GeneratorBase, IDungeonGenerator
{
    public Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel, Rarity rarity)
    {
        throw new NotImplementedException();
    }
}