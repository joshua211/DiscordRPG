using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class ItemUnequipped : AggregateEvent<CharacterAggregate, CharacterId>
{
    public ItemUnequipped(ItemId itemId)
    {
        ItemId = itemId;
    }

    public ItemId ItemId { get; private set; }
}