using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class DungeonDeleted : AggregateEvent<GuildAggregate, GuildId>
{
    public DungeonDeleted(DungeonId id)
    {
        Id = id;
    }

    public DungeonId Id { get; private set; }
}