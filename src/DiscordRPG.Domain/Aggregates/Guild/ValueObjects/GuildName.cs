using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

public class GuildName : ValueObject
{
    public GuildName(string value)
    {
        if (string.IsNullOrEmpty(value))
            DomainError.With(nameof(value));

        Value = value;
    }

    public string Value { get; private set; }
}