using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Activity.Commands;

public class AddActivityCommand : Command<GuildAggregate, GuildId>
{
    public AddActivityCommand(GuildId aggregateId, Activity activity) : base(aggregateId)
    {
        Activity = activity;
    }

    public Activity Activity { get; private set; }
}

public class AccActivityCommandHandler : CommandHandler<GuildAggregate, GuildId, AddActivityCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddActivityCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddActivity(command.Activity);
        return Task.CompletedTask;
    }
}