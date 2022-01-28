using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class CharacterName : ValueObject
{
    public CharacterName(string value)
    {
        if (string.IsNullOrEmpty(value))
            DomainError.With(nameof(value));

        Value = value;
    }

    public string Value { get; private set; }

    public override string ToString()
    {
        return Value;
    }
}