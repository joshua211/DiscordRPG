using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Activity.ValueObjects;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Entities.Activity;

public class Activity : Entity<ActivityId>
{
    public Activity(ActivityId id, ActivityDuration duration, ActivityType type, JobId jobId,
        ActivityData activityData) : base(id)
    {
        Duration = duration;
        Type = type;
        JobId = jobId;
        ActivityData = activityData;
    }

    public ActivityDuration Duration { get; private set; }
    public ActivityType Type { get; set; }
    public JobId JobId { get; set; }
    public ActivityData ActivityData { get; private set; }
}