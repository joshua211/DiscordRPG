using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Dungeon;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Domain.DomainServices;

public interface IAdventureResultService
{
    void Calculate(GuildAggregate aggregate, CharacterId characterId, DungeonId dungeonId, ActivityDuration duration,
        TransactionContext context);
}