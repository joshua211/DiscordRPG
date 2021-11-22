namespace DiscordRPG.Core.Entities;

public class Activity : Entity
{
    public Activity(ulong userId, DateTime startTime, TimeSpan duration, ActivityType type)
    {
        UserId = userId;
        StartTime = startTime;
        Duration = duration;
        Type = type;
    }

    public ulong UserId { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan Duration { get; private set; }
    public ActivityType Type { get; set; }
    public string JobId { get; set; }
}