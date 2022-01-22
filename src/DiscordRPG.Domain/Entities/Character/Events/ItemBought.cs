using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemBought : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public ItemBought(CharacterId entityId, Item item)
    {
        EntityId = entityId;
        Item = item;
    }

    public Item Item { get; private set; }
    public IEnumerable<string> EntityIds => new string[] {EntityId.Value, Item.Id.Value};

    public CharacterId EntityId { get; private set; }
}