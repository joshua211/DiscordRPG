namespace DiscordRPG.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventVersionAttribute : Attribute
{
    public EventVersionAttribute(int version)
    {
        Version = version;
    }

    public int Version { get; private set; }
}