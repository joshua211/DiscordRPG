using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class TitleEquipped : AggregateEvent<CharacterAggregate, CharacterId>
{
    public TitleEquipped(TitleId titleId)
    {
        TitleId = titleId;
    }

    public TitleId TitleId { get; private set; }
}