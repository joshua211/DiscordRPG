using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Activity.Commands;

public class CompleteActivityCommand : Command<ActivityAggregate, ActivityId>
{
    public CompleteActivityCommand(ActivityId aggregateId, ActivityId activityId, bool wasSuccessful,
        TransactionContext context) : base(aggregateId)
    {
        ActivityId = activityId;
        WasSuccessful = wasSuccessful;
        Context = context;
    }

    public ActivityId ActivityId { get; private set; }
    public TransactionContext Context { get; private set; }
    public bool WasSuccessful { get; private set; }
}

public class CompleteActivityCommandHandler : CommandHandler<ActivityAggregate, ActivityId, CompleteActivityCommand>
{
    public override Task ExecuteAsync(ActivityAggregate aggregate, CompleteActivityCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.CompleteActivity(command.ActivityId, command.WasSuccessful, command.Context);
        return Task.CompletedTask;
    }
}