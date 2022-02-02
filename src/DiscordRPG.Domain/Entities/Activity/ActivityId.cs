using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Activity;

public class ActivityId : Identity<ActivityId>
{
    public ActivityId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }
}