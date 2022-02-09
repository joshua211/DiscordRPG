using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;

public class Explorations : ValueObject
{
    public Explorations(byte value)
    {
        if (Value <= 0)
            DomainError.With("Explorations is already 0");
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