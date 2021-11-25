using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result<Character>> CreateCharacterAsync(ulong userId, ulong guildId, string name, Class characterClass,
        Race race, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteCharacterAsync(ulong userId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Character>> GetCharacterAsync(ulong userId, ulong guildId, TransactionContext parentContext = null,
        CancellationToken token = default);
}