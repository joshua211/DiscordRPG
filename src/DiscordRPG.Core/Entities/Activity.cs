namespace DiscordRPG.Core.Entities;

public class Activity : Entity
{
    public Activity(string charId, DateTime startTime, TimeSpan duration, ActivityType type, ActivityData data)
    {
        CharId = charId;
        StartTime = startTime;
        Duration = duration;
        Type = type;
        Data = data;
    }

    public string CharId { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan Duration { get; private set; }
    public ActivityType Type { get; set; }
    public string JobId { get; set; }
    public ActivityData Data { get; private set; }
}