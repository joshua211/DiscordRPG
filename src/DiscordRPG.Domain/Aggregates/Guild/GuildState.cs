﻿using DiscordRPG.Domain.Aggregates.Guild.Events;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Activity;
using DiscordRPG.Domain.Entities.Activity.Events;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Events;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Entities.Dungeon.Events;
using DiscordRPG.Domain.Entities.Shop;
using DiscordRPG.Domain.Entities.Shop.Events;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Guild;

public class GuildState : AggregateState<GuildAggregate, GuildId, GuildState>,
    IApply<GuildCreated>,
    IApply<GuildDeleted>,
    IApply<ActivityAdded>,
    IApply<ActivityCancelled>,
    IApply<ActivityCompleted>,
    IApply<CharacterCreated>,
    IApply<CharacterDied>,
    IApply<InventoryChanged>,
    IApply<ItemBought>,
    IApply<ItemEquipped>,
    IApply<ItemUnequipped>,
    IApply<LevelGained>,
    IApply<RestComplete>,
    IApply<WoundsChanged>,
    IApply<DungeonAdded>,
    IApply<DungeonDeleted>,
    IApply<ExplorationsDecreased>,
    IApply<ShopAdded>,
    IApply<ShopInventoryUpdated>,
    IApply<ShopInventoryRemoved>,
    IApply<AdventureResultCalculated>

{
    public GuildState()
    {
        Characters = new List<Character>();
        Dungeons = new List<Dungeon>();
        Activities = new List<Activity>();
        Shops = new List<Shop>();
    }

    public List<Character> Characters { get; private set; }
    public List<Dungeon> Dungeons { get; private set; }
    public List<Activity> Activities { get; private set; }
    public List<Shop> Shops { get; private set; }
    public GuildName GuildName { get; private set; }
    public ChannelId GuildHallId { get; private set; }
    public ChannelId DungeonHallId { get; private set; }
    public ChannelId InnId { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Apply(ActivityAdded aggregateEvent)
    {
        Activities.Add(aggregateEvent.Activity);
    }

    public void Apply(ActivityCancelled aggregateEvent)
    {
        Activities.RemoveAll(a => a.Id.Value == aggregateEvent.EntityId.Value);
    }

    public void Apply(ActivityCompleted aggregateEvent)
    {
        Activities.RemoveAll(a => a.Id.Value == aggregateEvent.EntityId.Value);
    }

    public void Apply(AdventureResultCalculated aggregateEvent)
    {
    }

    public void Apply(CharacterCreated aggregateEvent)
    {
        Characters.Add(aggregateEvent.Character);
    }

    public void Apply(CharacterDied aggregateEvent)
    {
        Characters.RemoveAll(c => c.Id.Value == aggregateEvent.EntityId.Value);
    }

    public void Apply(DungeonAdded aggregateEvent)
    {
        Dungeons.Add(aggregateEvent.Dungeon);
    }

    public void Apply(DungeonDeleted aggregateEvent)
    {
        Dungeons.RemoveAll(d => d.Id == aggregateEvent.EntityId);
    }

    public void Apply(ExplorationsDecreased aggregateEvent)
    {
        var dungeon = Dungeons.First(d => d.Id.Value == aggregateEvent.EntityId.Value);
        dungeon.DecreaseExplorations();
    }

    public void Apply(GuildCreated aggregateEvent)
    {
        GuildName = aggregateEvent.Name;
        GuildHallId = aggregateEvent.GuildHallId;
        DungeonHallId = aggregateEvent.DungeonHallId;
        InnId = aggregateEvent.InnId;
    }

    public void Apply(GuildDeleted aggregateEvent)
    {
        IsDeleted = true;
    }

    public void Apply(InventoryChanged aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.ChangeInventory(aggregateEvent.NewInventory);
    }

    public void Apply(ItemBought aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.AddMoney(-aggregateEvent.Item.Worth);
    }

    public void Apply(ItemEquipped aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.EquipItem(aggregateEvent.ItemId);
    }

    public void Apply(ItemUnequipped aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.UnequipItem(aggregateEvent.ItemId);
    }

    public void Apply(LevelGained aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.SetLevel(aggregateEvent.NewLevel);
    }

    public void Apply(RestComplete aggregateEvent)
    {
    }

    public void Apply(ShopAdded aggregateEvent)
    {
        Shops.Add(aggregateEvent.Shop);
    }

    public void Apply(ShopInventoryRemoved aggregateEvent)
    {
        var shop = Shops.First(s => s.Id.Value == aggregateEvent.EntityId.Value);
        shop.RemoveInventory(aggregateEvent.CharacterId);
    }

    public void Apply(ShopInventoryUpdated aggregateEvent)
    {
        var shop = Shops.First(s => s.Id.Value == aggregateEvent.EntityId.Value);
        shop.UpdateInventoryForCharacter(aggregateEvent.CharacterId, aggregateEvent.NewItems);
    }

    public void Apply(WoundsChanged aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.ChangeWounds(aggregateEvent.NewWounds);
    }
}