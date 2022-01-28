using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Activity.Commands;

public class CancelActivityCommand : Command<GuildAggregate, GuildId>
{
    public CancelActivityCommand(GuildId aggregateId, ActivityId activityId, TransactionContext context) :
        base(aggregateId)
    {
        ActivityId = activityId;
        Context = context;
    }

    public ActivityId ActivityId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CancelActivityCommandHandler : CommandHandler<GuildAggregate, GuildId, CancelActivityCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CancelActivityCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CancelActivity(command.ActivityId, command.Context);
        return Task.CompletedTask;
    }
}