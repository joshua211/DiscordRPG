using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

public class ChannelId : ValueObject
{
    public ChannelId(string value)
    {
        if (string.IsNullOrEmpty(value))
            DomainError.With(nameof(value));

        Value = value;
    }

    public string Value { get; private set; }
}