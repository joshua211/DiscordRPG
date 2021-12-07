using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CalculateAdventureResultCommand : Command
{
    public CalculateAdventureResultCommand(Character character, Dungeon dungeon, ActivityDuration duration)
    {
        Character = character;
        Dungeon = dungeon;
        Duration = duration;
    }

    public Character Character { get; private set; }
    public Dungeon Dungeon { get; private set; }
    public ActivityDuration Duration { get; private set; }
}