using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommand : Command
{
    public CreateDungeonCommand(Dungeon dungeon)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
}