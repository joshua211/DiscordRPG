using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class LevelGained : AggregateEvent<CharacterAggregate, CharacterId>
{
    public LevelGained(Level newLevel, Level oldLevel)
    {
        NewLevel = newLevel;
        OldLevel = oldLevel;
    }

    public Level NewLevel { get; private set; }
    public Level OldLevel { get; private set; }
}