using EventFlow.Core;

namespace DiscordRPG.Domain.Aggregates.Guild;

public class GuildId : IIdentity
{
    public GuildId(string value)
    {
        Value = value;
    }


    public string Value { get; }
}