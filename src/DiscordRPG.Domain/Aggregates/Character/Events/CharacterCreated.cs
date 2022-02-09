using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class CharacterCreated : AggregateEvent<CharacterAggregate, CharacterId>
{
    public CharacterCreated(CharacterClass @class, CharacterRace race, CharacterName name, Level characterLevel,
        List<Item> inventory, List<Wound> wounds, Money money, List<Recipe> knownRecipes, List<Title> titles)
    {
        Class = @class;
        Race = race;
        Name = name;
        CharacterLevel = characterLevel;
        Inventory = inventory;
        Wounds = wounds;
        Money = money;
        KnownRecipes = knownRecipes;
        Titles = titles;
    }

    public CharacterClass Class { get; }
    public CharacterRace Race { get; }
    public CharacterName Name { get; }
    public Level CharacterLevel { get; }
    public List<Item> Inventory { get; }
    public List<Wound> Wounds { get; }
    public Money Money { get; }
    public List<Recipe> KnownRecipes { get; }
    public List<Title> Titles { get; }
}