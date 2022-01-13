using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Activity.Commands;

public class AddActivityCommand : Command<GuildAggregate, GuildId>
{
    public AddActivityCommand(GuildId aggregateId, Activity activity, TransactionContext context) : base(aggregateId)
    {
        Activity = activity;
        Context = context;
    }

    public Activity Activity { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AccActivityCommandHandler : CommandHandler<GuildAggregate, GuildId, AddActivityCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddActivityCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddActivity(command.Activity, command.Context);
        return Task.CompletedTask;
    }
}