using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class ItemBought : AggregateEvent<CharacterAggregate, CharacterId>
{
    public ItemBought(Item item)
    {
        Item = item;
    }

    public Item Item { get; private set; }
}