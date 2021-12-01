namespace DiscordRPG.Application.Interfaces.Generators;

public interface IDungeonGenerator
{
    Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel, Rarity rarity);
}