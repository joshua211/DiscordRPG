using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class LearnRecipesCommand : Command<GuildAggregate, GuildId>
{
    public LearnRecipesCommand(GuildId aggregateId, CharacterId id, IEnumerable<Recipe> recipes,
        TransactionContext context) :
        base(aggregateId)
    {
        Id = id;
        Recipes = recipes;
        Context = context;
    }

    public CharacterId Id { get; private set; }
    public IEnumerable<Recipe> Recipes { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class LearnRecipeCommandHandler : CommandHandler<GuildAggregate, GuildId, LearnRecipesCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, LearnRecipesCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.LearnRecipes(command.Id, command.Recipes, command.Context);
        return Task.CompletedTask;
    }
}