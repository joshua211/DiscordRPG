using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class ForgeItemCommand : Command<GuildAggregate, GuildId>
{
    public ForgeItemCommand(GuildId aggregateId, CharacterId characterId, List<(ItemId id, int amount)> ingredients,
        TransactionContext context, EquipmentCategory category, uint level) : base(aggregateId)
    {
        CharacterId = characterId;
        Ingredients = ingredients;
        Context = context;
        Category = category;
        Level = level;
    }

    public CharacterId CharacterId { get; private set; }
    public uint Level { get; private set; }
    public List<(ItemId id, int amount)> Ingredients { get; private set; }
    public EquipmentCategory Category { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ForgeItemCommandHandler : CommandHandler<GuildAggregate, GuildId, ForgeItemCommand>
{
    private readonly IItemGenerator itemGenerator;

    public ForgeItemCommandHandler(IItemGenerator itemGenerator)
    {
        this.itemGenerator = itemGenerator;
    }

    public override Task ExecuteAsync(GuildAggregate aggregate, ForgeItemCommand command,
        CancellationToken cancellationToken)
    {
        var inventory = aggregate.Characters.First(c => c.Id == command.CharacterId).Inventory;
        var items = command.Ingredients.Select(ing => (inventory.First(i => i.Id == ing.id), ing.amount));
        //var items = inventory.Select(i => (i, command.Ingredients.First(ing => ing.id == i.Id).amount));
        var item = itemGenerator.ForgeItem(command.Category, command.Level, items);
        aggregate.ForgeItem(command.CharacterId, item, command.Ingredients, command.Context);

        return Task.CompletedTask;
    }
}