using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Dungeon;

public class DungeonId : IIdentity
{
    public DungeonId(string value)
    {
        Value = value;
    }

    public string Value { get; }
}