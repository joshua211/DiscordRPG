using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class ExplorationsDecreased : AggregateEvent<GuildAggregate, GuildId>
{
    public ExplorationsDecreased(DungeonId dungeonId)
    {
        DungeonId = dungeonId;
    }

    public DungeonId DungeonId { get; private set; }
}