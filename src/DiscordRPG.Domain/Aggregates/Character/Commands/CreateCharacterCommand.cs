using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class CreateCharacterCommand : Command<CharacterAggregate, CharacterId>
{
    public CreateCharacterCommand(CharacterId aggregateId, CharacterClass @class, CharacterRace race,
        CharacterName name,
        Level characterLevel, List<Item> inventory, List<Wound> wounds, Money money,
        List<Recipe> knownRecipes, List<Title> titles, TransactionContext context) :
        base(aggregateId)
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
        Context = context;
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
    public TransactionContext Context { get; private set; }
}

public class CreateCharacterCommandHandler : CommandHandler<CharacterAggregate, CharacterId, CreateCharacterCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CreateCharacter(command.Class, command.Race, command.Name, command.CharacterLevel, command.Inventory,
            command.Wounds, command.Money, command.KnownRecipes, command.Titles, command.Context);

        return Task.CompletedTask;
    }
}