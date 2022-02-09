using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class TitleAcquired : AggregateEvent<CharacterAggregate, CharacterId>
{
    public TitleAcquired(Title title)
    {
        Title = title;
    }

    public Title Title { get; private set; }
}