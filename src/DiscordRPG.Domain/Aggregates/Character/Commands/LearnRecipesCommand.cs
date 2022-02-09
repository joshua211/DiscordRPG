using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class LearnRecipesCommand : Command<CharacterAggregate, CharacterId>
{
    public LearnRecipesCommand(CharacterId aggregateId, IEnumerable<Recipe> recipes,
        TransactionContext context) :
        base(aggregateId)
    {
        Recipes = recipes;
        Context = context;
    }

    public IEnumerable<Recipe> Recipes { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class LearnRecipeCommandHandler : CommandHandler<CharacterAggregate, CharacterId, LearnRecipesCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, LearnRecipesCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.LearnRecipes(command.Id, command.Recipes, command.Context);
        return Task.CompletedTask;
    }
}