using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Dungeon.ValueObjects;

public class DungeonName : ValueObject
{
    public DungeonName(string value)
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