using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result<Character>> CreateCharacterAsync(DiscordId userId, Identity guildId, string name, Class characterClass,
        Race race, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Character>> GetCharacterAsync(Identity identity, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<Character>> GetUsersCharacterAsync(DiscordId userId, Identity guildId,
        TransactionContext parentContext = null,
        CancellationToken token = default);
}