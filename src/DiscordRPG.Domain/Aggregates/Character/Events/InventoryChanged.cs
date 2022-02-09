using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class InventoryChanged : AggregateEvent<CharacterAggregate, CharacterId>
{
    public InventoryChanged(List<Item> newInventory)
    {
        NewInventory = newInventory;
    }

    public List<Item> NewInventory { get; private set; }
}