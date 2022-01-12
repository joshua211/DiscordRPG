using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ShopInventoryRemoved : AggregateEvent<GuildAggregate, GuildId>
{
    public ShopInventoryRemoved(ShopId shopId, CharacterId characterId)
    {
        ShopId = shopId;
        CharacterId = characterId;
    }

    public ShopId ShopId { get; private set; }
    public CharacterId CharacterId { get; private set; }
}