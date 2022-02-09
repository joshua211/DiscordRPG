using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Activity.ValueObjects;
using DiscordRPG.Domain.Aggregates.Character;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Activity.Commands;

public class CreateActivityCommand : Command<ActivityAggregate, ActivityId>
{
    public CreateActivityCommand(ActivityId aggregateId, CharacterId characterId, JobId jobId,
        ActivityData activityData, ActivityStartTime startTime, ActivityDuration duration, ActivityType type,
        TransactionContext context) : base(
        aggregateId)
    {
        CharacterId = characterId;
        JobId = jobId;
        ActivityData = activityData;
        StartTime = startTime;
        Duration = duration;
        Type = type;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public ActivityDuration Duration { get; private set; }
    public ActivityType Type { get; set; }
    public JobId JobId { get; set; }
    public ActivityData ActivityData { get; private set; }
    public ActivityStartTime StartTime { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CreateActivityCommandHandler : CommandHandler<ActivityAggregate, ActivityId, CreateActivityCommand>
{
    public override Task ExecuteAsync(ActivityAggregate aggregate, CreateActivityCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.AddActivity(command.Activity, command.Context);
        return Task.CompletedTask;
    }
}