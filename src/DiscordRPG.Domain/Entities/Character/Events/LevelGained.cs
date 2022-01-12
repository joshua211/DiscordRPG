using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class LevelGained : AggregateEvent<GuildAggregate, GuildId>
{
    public LevelGained(CharacterId characterId, Level newLevel)
    {
        CharacterId = characterId;
        NewLevel = newLevel;
    }

    public CharacterId CharacterId { get; private set; }
    public Level NewLevel { get; private set; }
}