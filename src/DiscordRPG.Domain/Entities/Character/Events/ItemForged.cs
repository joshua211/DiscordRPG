using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemForged : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public ItemForged(CharacterId entityId, List<(ItemId id, int amount)> ingredients, Item item)
    {
        EntityId = entityId;
        Ingredients = ingredients;
        Item = item;
    }

    public List<(ItemId id, int amount)> Ingredients { get; private set; }
    public Item Item { get; private set; }
    public CharacterId EntityId { get; }
}