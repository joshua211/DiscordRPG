using DiscordRPG.Domain.Aggregates.Guild.Events;
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
using Serilog;

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
    IApply<ItemSold>,
    IApply<ItemEquipped>,
    IApply<ItemUnequipped>,
    IApply<LevelGained>,
    IApply<RestComplete>,
    IApply<WoundsChanged>,
    IApply<RecipesLearned>,
    IApply<DungeonAdded>,
    IApply<DungeonDeleted>,
    IApply<ExplorationsDecreased>,
    IApply<ShopAdded>,
    IApply<ShopInventoryUpdated>,
    IApply<ShopInventoryRemoved>,
    IApply<AdventureResultCalculated>,
    IApply<ItemRemoved>,
    IApply<TitleAcquired>,
    IApply<TitleEquipped>,
    IApply<TitleUnequipped>,
    IApply<ItemForged>

{
    private readonly ILogger logger;

    public GuildState(ILogger logger)
    {
        this.logger = logger;
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
        logger.Verbose("Added activity {Type} {Id} for {EntityId}", aggregateEvent.Activity.Type,
            aggregateEvent.Activity.Id, aggregateEvent.EntityId);
    }

    public void Apply(ActivityCancelled aggregateEvent)
    {
        Activities.RemoveAll(a => a.Id.Value == aggregateEvent.EntityId.Value);
        logger.Verbose("Cancelled activity {Id}", aggregateEvent.EntityId);
    }

    public void Apply(ActivityCompleted aggregateEvent)
    {
        Activities.RemoveAll(a => a.Id.Value == aggregateEvent.EntityId.Value);
        logger.Verbose("Completed activity {Id}", aggregateEvent.EntityId);
    }

    public void Apply(AdventureResultCalculated aggregateEvent)
    {
        logger.Verbose("Calculated adventure result from dungeon {DId} for character {CId}: {@Result}",
            aggregateEvent.DungeonId, aggregateEvent.CharacterId, aggregateEvent.AdventureResult);
    }

    public void Apply(CharacterCreated aggregateEvent)
    {
        Characters.Add(aggregateEvent.Character);
        logger.Verbose("Created character {Name} for Guild {GName}", aggregateEvent.Character.Name, GuildName);
    }

    public void Apply(CharacterDied aggregateEvent)
    {
        Characters.RemoveAll(c => c.Id.Value == aggregateEvent.EntityId.Value);
        logger.Verbose("{Id} died", aggregateEvent.EntityId);
    }

    public void Apply(DungeonAdded aggregateEvent)
    {
        Dungeons.Add(aggregateEvent.Dungeon);
        logger.Verbose("Added Dungeon {@Dungeon} to {Name}", aggregateEvent.Dungeon, GuildName);
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
        character.Inventory.Add(aggregateEvent.Item);
    }

    public void Apply(ItemEquipped aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.EquipItem(aggregateEvent.ItemId);
    }

    public void Apply(ItemForged aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        foreach (var ingredient in aggregateEvent.Ingredients)
        {
            var item = character.Inventory.First(i => i.Id == ingredient.id);
            if (item.Amount > ingredient.amount)
                item.IncreaseAmount(-ingredient.amount);
            else
                character.Inventory.Remove(item);
        }

        character.Inventory.Add(aggregateEvent.Item);
    }

    public void Apply(ItemRemoved aggregateEvent)
    {
        var shop = Shops.First();
        var playerSellInv = shop.Inventory.First(i => i.CharacterId == aggregateEvent.CharacterId);
        var item = playerSellInv.ItemsForSale.First(i => i.Id == aggregateEvent.ItemId);
        playerSellInv.ItemsForSale.Remove(item);
    }

    public void Apply(ItemSold aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        var item = character.Inventory.FirstOrDefault(i => i.Id == aggregateEvent.ItemId);

        character.AddMoney((int) (item.Worth * item.Amount * 0.7f)); //TODO calculate sell price modificator
        character.Inventory.Remove(item);
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
        logger.Verbose("{Name} gained Level {@Level}", aggregateEvent.EntityId, aggregateEvent.NewLevel);
    }

    public void Apply(RecipesLearned aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.LearnRecipes(aggregateEvent.Recipes);
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

    public void Apply(TitleAcquired aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.Titles.Add(aggregateEvent.Title);
        logger.Verbose("{Name} gained title {Title}", character.Name, aggregateEvent.Title);
    }

    public void Apply(TitleEquipped aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        var existing = character.Titles.FirstOrDefault(t => t.IsEquipped);
        if (existing is not null)
            existing.Unequip();

        var title = character.Titles.FirstOrDefault(t => t.Id == aggregateEvent.TitleId);
        title.Equip();
    }

    public void Apply(TitleUnequipped aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        var existing = character.Titles.FirstOrDefault(t => t.IsEquipped);
        if (existing is not null)
            existing.Unequip();
    }

    public void Apply(WoundsChanged aggregateEvent)
    {
        var character = Characters.First(c => c.Id.Value == aggregateEvent.EntityId.Value);
        character.ChangeWounds(aggregateEvent.NewWounds);
        logger.Verbose("{Name} changed wound dmg to {NewHp}", character.Name, character.Wounds.Sum(w => w.DamageValue));
    }
}