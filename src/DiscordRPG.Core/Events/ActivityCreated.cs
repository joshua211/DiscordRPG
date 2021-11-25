using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class ActivityCreated : DomainEvent
{
    public ActivityCreated(Activity activity)
    {
        Activity = activity;
    }

    public Activity Activity { get; private set; }
}