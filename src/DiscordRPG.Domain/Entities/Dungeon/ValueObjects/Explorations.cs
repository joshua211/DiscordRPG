using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Dungeon.ValueObjects;

public class Explorations : ValueObject
{
    public Explorations(byte value)
    {
        if (value <= 0)
            DomainError.With(nameof(value));
        Value = value;
    }

    public byte Value { get; private set; }

    public Explorations Decrease()
    {
        if (Value == 0)
            throw new ArgumentException(nameof(Value));

        return new Explorations(--Value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}