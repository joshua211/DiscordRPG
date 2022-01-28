using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Activity.ValueObjects;

public class ActivityStartTime : ValueObject
{
    public ActivityStartTime(DateTime value)
    {
        Value = value;
    }

    public DateTime Value { get; private set; }
}