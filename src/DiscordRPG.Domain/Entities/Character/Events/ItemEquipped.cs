﻿using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemEquipped : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public ItemEquipped(ItemId itemId, CharacterId entityId)
    {
        ItemId = itemId;
        EntityId = entityId;
    }

    public ItemId ItemId { get; private set; }
    public CharacterId EntityId { get; private set; }
}