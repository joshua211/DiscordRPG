using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Activity.Commands;

public class CancelActivityCommand : Command<ActivityAggregate, ActivityId>
{
    public CancelActivityCommand(ActivityId aggregateId, ActivityId activityId, TransactionContext context) :
        base(aggregateId)
    {
        ActivityId = activityId;
        Context = context;
    }

    public ActivityId ActivityId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CancelActivityCommandHandler : CommandHandler<ActivityAggregate, ActivityId, CancelActivityCommand>
{
    public override Task ExecuteAsync(ActivityAggregate aggregate, CancelActivityCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.CancelActivity(command.ActivityId, command.Context);
        return Task.CompletedTask;
    }
}