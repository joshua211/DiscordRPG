using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class DungeonAdded : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<DungeonId>
{
    public DungeonAdded(Dungeon dungeon)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
    public DungeonId EntityId => Dungeon.Id;
}