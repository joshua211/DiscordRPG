using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Activity.ValueObjects;

public class JobId : ValueObject
{
    public JobId(string value)
    {
        if (string.IsNullOrEmpty(value))
            DomainError.With(nameof(value));
        Value = value;
    }

    public string Value { get; private set; }
}