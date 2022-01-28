using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class LevelGained : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public LevelGained(CharacterId entityId, Level newLevel, Level oldLevel)
    {
        EntityId = entityId;
        NewLevel = newLevel;
        OldLevel = oldLevel;
    }

    public Level NewLevel { get; private set; }
    public Level OldLevel { get; private set; }
    public CharacterId EntityId { get; private set; }
}