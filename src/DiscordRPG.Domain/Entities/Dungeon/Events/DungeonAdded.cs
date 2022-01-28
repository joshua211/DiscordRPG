using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class DungeonAdded : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<DungeonId>
{
    public DungeonAdded(Dungeon dungeon, CharacterId characterId)
    {
        Dungeon = dungeon;
        CharacterId = characterId;
    }

    public Dungeon Dungeon { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public DungeonId EntityId => Dungeon.Id;
}