using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Guild.Events;

public class AdventureResultCalculated : AggregateEvent<GuildAggregate, GuildId>
{
    public AdventureResultCalculated(AdventureResult adventureResult, CharacterId characterId, DungeonId dungeonId)
    {
        AdventureResult = adventureResult;
        CharacterId = characterId;
        DungeonId = dungeonId;
    }

    public AdventureResult AdventureResult { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public DungeonId DungeonId { get; private set; }
}