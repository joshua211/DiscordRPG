using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Commands.Base;

public class GuildCommandContext
{
    public GuildCommandContext(Character? character, Activity? activity, Dungeon? dungeon)
    {
        Character = character;
        Activity = activity;
        Dungeon = dungeon;
    }

    public Character? Character { get; private set; }
    public Activity? Activity { get; private set; }
    public Dungeon? Dungeon { get; private set; }
}