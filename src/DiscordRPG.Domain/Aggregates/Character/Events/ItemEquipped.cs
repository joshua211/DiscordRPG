using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class ItemEquipped : AggregateEvent<CharacterAggregate, CharacterId>
{
    public ItemEquipped(ItemId itemId)
    {
        ItemId = itemId;
    }

    public ItemId ItemId { get; private set; }
}