using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class InventoryChanged : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public InventoryChanged(CharacterId entityId, List<Item> newInventory)
    {
        EntityId = entityId;
        NewInventory = newInventory;
    }

    public List<Item> NewInventory { get; private set; }

    public CharacterId EntityId { get; private set; }
}