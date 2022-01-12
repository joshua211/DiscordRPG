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

    public void Create(GuildId id, GuildName name, ChannelId guildHallId, ChannelId dungeonHallId, ChannelId innId)
    {
        Emit(new GuildCreated(id, name, guildHallId, dungeonHallId, innId));
    }

    public void Delete()
    {
        Emit(new GuildDeleted());
    }

    public void AddActivity(Activity activity)
    {
        //TODO spec
        Emit(new ActivityAdded(activity));
    }

    public void CompleteActivity(ActivityId id, bool success)
    {
        //TODO spec
        Emit(new ActivityCompleted(id, success));
    }

    public void CancelActivity(ActivityId commandActivityId)
    {
        //TODO spec
        Emit(new ActivityCancelled(commandActivityId));
    }

    public void AddCharacter(Character commandCharacter)
    {
        Emit(new CharacterCreated(commandCharacter));
    }

    public void KillCharacter(CharacterId commandCharacterId)
    {
        Emit(new CharacterDied(commandCharacterId));
    }

    public void ChangeCharacterInventory(CharacterId commandCharacterId, List<Item> commandNewInventory)
    {
        Emit(new InventoryChanged(commandCharacterId, commandNewInventory));
    }

    public void EquipItem(CharacterId commandCharacterId, ItemId commandItemId)
    {
        Emit(new ItemEquipped(commandItemId, commandCharacterId));
    }

    public void UnequipItem(CharacterId commandCharacterId, ItemId commandItemId)
    {
        Emit(new ItemUnequipped(commandItemId, commandCharacterId));
    }

    public void SetCharacterLevel(CharacterId commandCharacterId, Level commandLevel)
    {
        Emit(new LevelGained(commandCharacterId, commandLevel));
    }

    public void CompleteCharacterRest(CharacterId commandCharacterId)
    {
        //TODO change wounds after rest complete
        Emit(new RestComplete(commandCharacterId));
    }

    public void AddDungeon(Dungeon commandDungeon)
    {
        Emit(new DungeonAdded(commandDungeon));
    }

    public void RemoveDungeon(DungeonId commandDungeonId)
    {
        Emit(new DungeonDeleted(commandDungeonId));
    }

    public void DecreaseExplorations(DungeonId commandDungeonId)
    {
        Emit(new ExplorationsDecreased(commandDungeonId));
    }

    public void AddShop(Shop commandShop)
    {
        Emit(new ShopAdded(commandShop));
    }

    public void UpdateShopInventory(ShopId commandShopId, List<Item> commandNewInventory,
        CharacterId commandCharacterId)
    {
        Emit(new ShopInventoryUpdated(commandShopId, commandCharacterId, commandNewInventory));
    }

    public void BuyItem(CharacterId commandCharacterId, Item item)
    {
        Emit(new ItemBought(commandCharacterId, item));
    }

    public void RemoveShopInventory(ShopId shopId, CharacterId charId)
    {
        Emit(new ShopInventoryRemoved(shopId, charId));
    }
}