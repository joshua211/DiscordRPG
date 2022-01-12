using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemBought : AggregateEvent<GuildAggregate, GuildId>
{
    public ItemBought(CharacterId characterId, Item item)
    {
        CharacterId = characterId;
        Item = item;
    }

    public CharacterId CharacterId { get; private set; }
    public Item Item { get; private set; }
}