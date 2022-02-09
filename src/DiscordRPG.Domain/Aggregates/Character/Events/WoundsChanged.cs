using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class WoundsChanged : AggregateEvent<CharacterAggregate, CharacterId>
{
    public WoundsChanged(List<Wound> newWounds)
    {
        NewWounds = newWounds;
    }

    public List<Wound> NewWounds { get; private set; }
}