namespace DiscordRPG.Core.Entities;

public class Activity : Entity
{
    public Activity(Identity charId, DateTime startTime, ActivityDuration duration, ActivityType type,
        ActivityData data)
    {
        CharId = charId;
        StartTime = startTime;
        Duration = duration;
        Type = type;
        Data = data;
    }

    public Identity CharId { get; private set; }
    public DateTime StartTime { get; private set; }
    public ActivityDuration Duration { get; private set; }
    public ActivityType Type { get; set; }
    public string JobId { get; set; }
    public ActivityData Data { get; private set; }
}