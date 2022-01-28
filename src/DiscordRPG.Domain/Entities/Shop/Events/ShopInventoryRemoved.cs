using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ShopInventoryRemoved : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ShopId>
{
    public ShopInventoryRemoved(ShopId entityId, CharacterId characterId)
    {
        EntityId = entityId;
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }

    public ShopId EntityId { get; private set; }
}