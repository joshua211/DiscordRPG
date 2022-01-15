using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Money : ValueObject
{
    public Money(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }

    public Money Add(int amount)
    {
        return new Money(Value + amount);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}