using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class CraftItemCommand : Command<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public CraftItemCommand(GuildId aggregateId, CharacterId entityId, RecipeId recipeId, TransactionContext context) :
        base(aggregateId)
    {
        EntityId = entityId;
        RecipeId = recipeId;
        Context = context;
    }

    public RecipeId RecipeId { get; private set; }
    public TransactionContext Context { get; private set; }

    public CharacterId EntityId { get; }
}

public class CraftItemCommandHandler : CommandHandler<GuildAggregate, GuildId, CraftItemCommand>
{
    private readonly ICraftingService craftingService;

    public CraftItemCommandHandler(ICraftingService craftingService)
    {
        this.craftingService = craftingService;
    }

    public override Task ExecuteAsync(GuildAggregate aggregate, CraftItemCommand command,
        CancellationToken cancellationToken)
    {
        craftingService.CraftItem(aggregate, command.EntityId, command.RecipeId, command.Context);

        return Task.CompletedTask;
    }
}