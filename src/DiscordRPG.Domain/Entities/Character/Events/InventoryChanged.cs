using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class InventoryChanged : AggregateEvent<GuildAggregate, GuildId>
{
    public InventoryChanged(CharacterId characterId, List<Item> newInventory)
    {
        CharacterId = characterId;
        NewInventory = newInventory;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Item> NewInventory { get; private set; }
}