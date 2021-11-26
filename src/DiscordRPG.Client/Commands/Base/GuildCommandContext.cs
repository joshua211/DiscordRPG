using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Commands.Base;

public class GuildCommandContext
{
    public GuildCommandContext(Character? character, Activity? activity, Dungeon? dungeon, Guild? guild)
    {
        Character = character;
        Activity = activity;
        Dungeon = dungeon;
        Guild = guild;
    }

    public Character? Character { get; private set; }
    public Activity? Activity { get; private set; }
    public Dungeon? Dungeon { get; private set; }
    public Guild? Guild { get; private set; }
}