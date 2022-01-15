using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Activity.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Entities.Activity;

public class Activity : Entity<ActivityId>
{
    public Activity(ActivityId id, ActivityDuration duration, ActivityType type, JobId jobId,
        ActivityData activityData, CharacterId characterId, ActivityStartTime startTime) : base(id)
    {
        Duration = duration;
        Type = type;
        JobId = jobId;
        ActivityData = activityData;
        CharacterId = characterId;
        StartTime = startTime;
    }

    public CharacterId CharacterId { get; private set; }
    public ActivityDuration Duration { get; private set; }
    public ActivityType Type { get; set; }
    public JobId JobId { get; set; }
    public ActivityData ActivityData { get; private set; }
    public ActivityStartTime StartTime { get; private set; }
}