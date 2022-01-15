using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;

namespace DiscordRPG.Domain.DomainServices;

public interface IAdventureResultService
{
    void Calculate(GuildAggregate aggregate, CharacterId characterId, DungeonId dungeonId, ActivityDuration duration,
        TransactionContext context);
}