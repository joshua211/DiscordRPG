using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class ItemSold : AggregateEvent<CharacterAggregate, CharacterId>
{
    public ItemSold(ItemId itemId)
    {
        ItemId = itemId;
    }

    public ItemId ItemId { get; private set; }
}