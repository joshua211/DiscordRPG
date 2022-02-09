using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class TitleUnequipped : AggregateEvent<CharacterAggregate, CharacterId>
{
    public TitleUnequipped(TitleId titleId)
    {
        TitleId = titleId;
    }

    public TitleId TitleId { get; private set; }
}