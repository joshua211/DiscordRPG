using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class LearnRecipeCommand : Command<GuildAggregate, GuildId>
{
    public LearnRecipeCommand(GuildId aggregateId, CharacterId id, Recipe recipe, TransactionContext context) :
        base(aggregateId)
    {
        Id = id;
        Recipe = recipe;
        Context = context;
    }

    public CharacterId Id { get; private set; }
    public Recipe Recipe { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class LearnRecipeCommandHandler : CommandHandler<GuildAggregate, GuildId, LearnRecipeCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, LearnRecipeCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.LearnRecipe(command.Id, command.Recipe, command.Context);
        return Task.CompletedTask;
    }
}