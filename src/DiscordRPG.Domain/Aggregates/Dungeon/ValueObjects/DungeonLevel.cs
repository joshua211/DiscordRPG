using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;

public class DungeonLevel : ValueObject
{
    public DungeonLevel(uint value)
    {
        Value = value;
    }

    public uint Value { get; private set; }

    public override string ToString()
    {
        return Value.ToString();
    }
}