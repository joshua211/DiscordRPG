using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Events;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character;

public class CharacterAggregate : AggregateRoot<CharacterAggregate, CharacterId>
{
    private readonly CharacterState state;

    public CharacterAggregate(CharacterId id) : base(id)
    {
        state = new CharacterState();
        Register(state);
    }


    public void CreateCharacter(CharacterClass @class, CharacterRace race, CharacterName name, Level level,
        List<Item> inventory, List<Wound> wounds, Money money,
        List<Recipe> recipes, List<Title> titles, TransactionContext context)
    {
        Emit(new CharacterCreated(@class, race, name, level, inventory, wounds, money, recipes, titles),
            new Metadata(context.AsMetadata()));
    }

    public void EquipItem(ItemId itemId, TransactionContext context)
    {
        Emit(new ItemEquipped(itemId), new Metadata(context.AsMetadata()));
    }

    public void AddTitle(Title title, TransactionContext context)
    {
        Emit(new TitleAcquired(title), new Metadata(context.AsMetadata()));
    }

    public void ChangeCharacterInventory(List<Item> inventory, TransactionContext context)
    {
        Emit(new InventoryChanged(inventory), new Metadata(context.AsMetadata()));
    }

    public void ChangeWounds(List<Wound> wounds, TransactionContext context)
    {
        Emit(new WoundsChanged(wounds), new Metadata(context.AsMetadata()));
    }
}