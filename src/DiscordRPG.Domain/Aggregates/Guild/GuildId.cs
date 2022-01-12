using EventFlow.Core;

namespace DiscordRPG.Domain.Aggregates.Guild;

public class GuildId : Identity<GuildId>
{
    public GuildId(string value) : base(value)
    {
        Value = value;
    }

    public string Value { get; }
}