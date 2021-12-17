using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommand : Command
{
    public CreateDungeonCommand(Dungeon dungeon, Character character, ActivityDuration activityDuration)
    {
        Dungeon = dungeon;
        Character = character;
        ActivityDuration = activityDuration;
    }

    public Dungeon Dungeon { get; private set; }
    public Character Character { get; private set; }
    public ActivityDuration ActivityDuration { get; }
}