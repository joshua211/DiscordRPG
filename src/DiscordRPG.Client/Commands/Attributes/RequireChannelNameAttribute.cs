namespace DiscordRPG.Client.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RequireChannelNameAttribute : Attribute
{
    public RequireChannelNameAttribute(string channelName)
    {
        ChannelName = channelName;
    }

    public string ChannelName { get; private set; }
}