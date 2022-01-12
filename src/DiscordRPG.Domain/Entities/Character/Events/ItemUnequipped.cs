using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemUnequipped : AggregateEvent<GuildAggregate, GuildId>
{
    public ItemUnequipped(ItemId itemId, CharacterId characterId)
    {
        ItemId = itemId;
        CharacterId = characterId;
    }

    public ItemId ItemId { get; private set; }
    public CharacterId CharacterId { get; private set; }
}