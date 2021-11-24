using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Guilds;

public class AddDungeonCommand : Command
{
    public AddDungeonCommand(Dungeon dungeon, Guild guild)
    {
        Dungeon = dungeon;
        Guild = guild;
    }

    public Dungeon Dungeon { get; private set; }
    public Guild Guild { get; private set; }
}