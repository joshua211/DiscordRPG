using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result<Character>> CreateCharacterAsync(ulong userId, ulong guildId, string name, Class characterClass,
        Race race,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteCharacterAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<Result<Character>> GetCharacterAsync(ulong userId, ulong guildId, CancellationToken token = default);
}