using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.DomainServices.Generators;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class ForgeItemCommand : Command<CharacterAggregate, CharacterId>
{
    public ForgeItemCommand(CharacterId aggregateId,
        List<(ItemId id, int amount)> ingredients,
        TransactionContext context, EquipmentCategory category, uint level) : base(aggregateId)
    {
        Ingredients = ingredients;
        Context = context;
        Category = category;
        Level = level;
    }

    public uint Level { get; private set; }
    public List<(ItemId id, int amount)> Ingredients { get; private set; }
    public EquipmentCategory Category { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ForgeItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, ForgeItemCommand>
{
    private readonly IItemGenerator itemGenerator;

    public ForgeItemCommandHandler(IItemGenerator itemGenerator)
    {
        this.itemGenerator = itemGenerator;
    }

    public override Task ExecuteAsync(CharacterAggregate aggregate, ForgeItemCommand command,
        CancellationToken cancellationToken)
    {
        /*var inventory = aggregate.Characters.First(c => c.Id == command.CharacterId).Inventory;
        var items = command.Ingredients.Select(ing => (inventory.First(i => i.Id == ing.id), ing.amount));
        //var items = inventory.Select(i => (i, command.Ingredients.First(ing => ing.id == i.Id).amount));
        var item = itemGenerator.ForgeItem(command.Category, command.Level, items);
        aggregate.ForgeItem(command.CharacterId, item, command.Ingredients, command.Context);*/

        return Task.CompletedTask;
    }
}