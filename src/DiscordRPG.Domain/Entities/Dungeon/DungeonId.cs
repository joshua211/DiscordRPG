using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Dungeon;

public class DungeonId : Identity<DungeonId>
{
    public DungeonId(string value) : base(value)
    {
    }
}