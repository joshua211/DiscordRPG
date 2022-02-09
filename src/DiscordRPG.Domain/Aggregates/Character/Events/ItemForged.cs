using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class ItemForged : AggregateEvent<CharacterAggregate, CharacterId>
{
    public ItemForged(List<(ItemId id, int amount)> ingredients, Item item)
    {
        Ingredients = ingredients;
        Item = item;
    }

    public List<(ItemId id, int amount)> Ingredients { get; private set; }
    public Item Item { get; private set; }
}