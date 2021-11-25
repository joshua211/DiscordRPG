namespace DiscordRPG.Core.Events;

public class ActivityDeleted : DomainEvent
{
    public ActivityDeleted(string activityId)
    {
        ActivityId = activityId;
    }

    public string ActivityId { get; private set; }
}