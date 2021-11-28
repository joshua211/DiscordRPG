using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CalculateAdventureResultCommand : Command
{
    public CalculateAdventureResultCommand(Character character, Dungeon dungeon)
    {
        Character = character;
        Dungeon = dungeon;
    }

    public Character Character { get; private set; }
    public Dungeon Dungeon { get; private set; }
}