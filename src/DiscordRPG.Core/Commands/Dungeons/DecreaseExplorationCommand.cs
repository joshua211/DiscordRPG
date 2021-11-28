using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class DecreaseExplorationCommand : Command
{
    public DecreaseExplorationCommand(Dungeon dungeon)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
}