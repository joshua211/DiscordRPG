using EventFlow.Core;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Dungeon;

public class DungeonId : SingleValueObject<string>, IIdentity
{
    public DungeonId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }
}