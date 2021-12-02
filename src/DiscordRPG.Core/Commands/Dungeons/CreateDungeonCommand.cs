using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommand : Command
{
    public CreateDungeonCommand(Dungeon dungeon, Character character)
    {
        Dungeon = dungeon;
        Character = character;
    }

    public Dungeon Dungeon { get; private set; }
    public Character Character { get; private set; }
}