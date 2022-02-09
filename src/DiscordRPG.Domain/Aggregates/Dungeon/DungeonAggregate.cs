using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Dungeon;

public class DungeonAggregate : AggregateRoot<DungeonAggregate, DungeonId>
{
    public DungeonAggregate(DungeonId id) : base(id)
    {
    }
}