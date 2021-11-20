using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result<Character>> CreateCharacterAsync(ulong userId, string name, Class characterClass, Race race,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteCharacterAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<Result<Character>> GetCharacterAsync(ulong userId, CancellationToken token = default);
}