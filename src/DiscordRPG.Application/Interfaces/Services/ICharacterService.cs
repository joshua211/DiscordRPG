using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result> CreateCharacterAsync(CharacterId characterId, GuildId guildId, string name,
        CharacterClass characterClass,
        CharacterRace race, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result<CharacterReadModel>> GetCharacterAsync(CharacterId id, TransactionContext context,
        CancellationToken token = default);

    Task<Result> RestoreWoundsFromRestAsync(GuildId guildId, CharacterId charId, ActivityDuration activityDuration,
        TransactionContext context,
        CancellationToken token = default);

    Task<Result<IEnumerable<CharacterReadModel>>> GetAllCharactersInGuild(GuildId guildId,
        TransactionContext context,
        CancellationToken cancellationToken = default);
}