namespace DiscordRPG.Client.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HandlerAttribute : Attribute
{
    public HandlerAttribute(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}