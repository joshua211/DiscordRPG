using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.DomainServices;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class CraftItemCommand : Command<CharacterAggregate, CharacterId>
{
    public CraftItemCommand(CharacterId aggregateId, RecipeId recipeId, TransactionContext context) :
        base(aggregateId)
    {
        RecipeId = recipeId;
        Context = context;
    }

    public RecipeId RecipeId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CraftItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, CraftItemCommand>
{
    private readonly ICraftingService craftingService;

    public CraftItemCommandHandler(ICraftingService craftingService)
    {
        this.craftingService = craftingService;
    }

    public override Task ExecuteAsync(CharacterAggregate aggregate, CraftItemCommand command,
        CancellationToken cancellationToken)
    {
        craftingService.CraftItem(command.RecipeId, command.Context);

        return Task.CompletedTask;
    }
}