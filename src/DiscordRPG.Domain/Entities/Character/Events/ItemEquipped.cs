﻿using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemEquipped : AggregateEvent<GuildAggregate, GuildId>
{
    public ItemEquipped(ItemId itemId, CharacterId characterId)
    {
        ItemId = itemId;
        CharacterId = characterId;
    }

    public ItemId ItemId { get; private set; }
    public CharacterId CharacterId { get; private set; }
}