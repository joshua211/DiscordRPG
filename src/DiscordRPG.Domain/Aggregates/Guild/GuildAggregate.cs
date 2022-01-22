using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild.Events;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Activity;
using DiscordRPG.Domain.Entities.Activity.Events;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Events;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Entities.Dungeon.Events;
using DiscordRPG.Domain.Entities.Shop;
using DiscordRPG.Domain.Entities.Shop.Events;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Guild;

public class GuildAggregate : AggregateRoot<GuildAggregate, GuildId>
{
    private readonly GuildState state;

    public GuildAggregate(GuildId id) : base(id)
    {
        state = new GuildState();
        Register(state);
    }

    public IEnumerable<Character> Characters => state.Characters;
    public IEnumerable<Dungeon> Dungeons => state.Dungeons;
    public Shop Shop => state.Shops.First();

    public void Create(GuildId id, GuildName name, ChannelId guildHallId, ChannelId dungeonHallId, ChannelId innId,
        TransactionContext context)
    {
        Emit(new GuildCreated(id, name, guildHallId, dungeonHallId, innId), new Metadata(context.AsMetadata()));
    }

    public void Delete(TransactionContext context)
    {
        Emit(new GuildDeleted(), new Metadata(context.AsMetadata()));
    }

    public void AddActivity(Activity activity, TransactionContext context)
    {
        //TODO spec
        Emit(new ActivityAdded(activity), new Metadata(context.AsMetadata()));
    }

    public void CompleteActivity(ActivityId id, bool success, TransactionContext context)
    {
        //TODO spec
        Emit(new ActivityCompleted(id, success), new Metadata(context.AsMetadata()));
    }

    public void CancelActivity(ActivityId commandActivityId, TransactionContext context)
    {
        //TODO spec
        Emit(new ActivityCancelled(commandActivityId), new Metadata(context.AsMetadata()));
    }

    public void AddCharacter(Character commandCharacter, TransactionContext context)
    {
        Emit(new CharacterCreated(commandCharacter), new Metadata(context.AsMetadata()));
    }

    public void KillCharacter(CharacterId commandCharacterId, TransactionContext context)
    {
        Emit(new CharacterDied(commandCharacterId), new Metadata(context.AsMetadata()));
    }

    public void ChangeCharacterInventory(CharacterId commandCharacterId, List<Item> commandNewInventory,
        TransactionContext context)
    {
        Emit(new InventoryChanged(commandCharacterId, commandNewInventory), new Metadata(context.AsMetadata()));
    }

    public void EquipItem(CharacterId commandCharacterId, ItemId commandItemId, TransactionContext context)
    {
        Emit(new ItemEquipped(commandItemId, commandCharacterId), new Metadata(context.AsMetadata()));
    }

    public void UnequipItem(CharacterId commandCharacterId, ItemId commandItemId, TransactionContext context)
    {
        Emit(new ItemUnequipped(commandItemId, commandCharacterId), new Metadata(context.AsMetadata()));
    }

    public void SetCharacterLevel(CharacterId commandCharacterId, Level newLevel, Level oldLevel,
        TransactionContext context)
    {
        Emit(new LevelGained(commandCharacterId, newLevel, oldLevel), new Metadata(context.AsMetadata()));
    }

    public void CompleteCharacterRest(CharacterId commandCharacterId, TransactionContext context)
    {
        //TODO change wounds after rest complete
        Emit(new RestComplete(commandCharacterId), new Metadata(context.AsMetadata()));
    }

    public void AddDungeon(Dungeon commandDungeon, TransactionContext context)
    {
        Emit(new DungeonAdded(commandDungeon), new Metadata(context.AsMetadata()));
    }

    public void RemoveDungeon(DungeonId commandDungeonId, TransactionContext context)
    {
        Emit(new DungeonDeleted(commandDungeonId), new Metadata(context.AsMetadata()));
    }

    public void DecreaseExplorations(DungeonId commandDungeonId, TransactionContext context)
    {
        Emit(new ExplorationsDecreased(commandDungeonId), new Metadata(context.AsMetadata()));
    }

    public void AddShop(Shop commandShop, TransactionContext context)
    {
        Emit(new ShopAdded(commandShop), new Metadata(context.AsMetadata()));
    }

    public void UpdateShopInventory(ShopId commandShopId, List<Item> commandNewInventory,
        CharacterId commandCharacterId, TransactionContext context)
    {
        Emit(new ShopInventoryUpdated(commandShopId, commandCharacterId, commandNewInventory),
            new Metadata(context.AsMetadata()));
    }

    public void BuyItem(CharacterId characterId, ItemId itemId, TransactionContext context)
    {
        var shopInventory = Shop.Inventory.First(s => s.CharacterId == characterId);
        var item = shopInventory.ItemsForSale.First(i => i.Id == itemId);
        Emit(new ItemBought(characterId, item), new Metadata(context.AsMetadata()));
        Emit(new ItemRemoved(characterId, itemId, Shop.Id));
    }

    public void RemoveShopInventory(ShopId shopId, CharacterId charId, TransactionContext context)
    {
        Emit(new ShopInventoryRemoved(shopId, charId), new Metadata(context.AsMetadata()));
    }

    public void PublishAdventureResult(AdventureResult result, CharacterId characterId, DungeonId dungeonId,
        TransactionContext context)
    {
        Emit(new AdventureResultCalculated(result, characterId, dungeonId), new Metadata(context.AsMetadata()));
    }

    public void ChangeWounds(CharacterId characterId, List<Wound> wounds, TransactionContext context)
    {
        Emit(new WoundsChanged(characterId, wounds), new Metadata(context.AsMetadata()));
    }

    public void LearnRecipes(CharacterId id, IEnumerable<Recipe> recipes, TransactionContext context)
    {
        Emit(new RecipesLearned(id, recipes), new Metadata(context.AsMetadata()));
    }

    public void SellItem(CharacterId entityId, ItemId itemId, TransactionContext context)
    {
        Emit(new ItemSold(entityId, itemId), new Metadata(context.AsMetadata()));
    }
}