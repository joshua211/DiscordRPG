using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Activity.Commands;

public class CompleteActivityCommand : Command<GuildAggregate, GuildId>
{
    public CompleteActivityCommand(GuildId aggregateId, ActivityId activityId, bool wasSuccessful) : base(aggregateId)
    {
        ActivityId = activityId;
        WasSuccessful = wasSuccessful;
    }

    public ActivityId ActivityId { get; private set; }
    public bool WasSuccessful { get; private set; }
}

public class CompleteActivityCommandHandler : CommandHandler<GuildAggregate, GuildId, CompleteActivityCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CompleteActivityCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CompleteActivity(command.ActivityId, command.WasSuccessful);
        return Task.CompletedTask;
    }
}