using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface ICharacterRepository
{
    Task<Character> GetGuildCharacterAsync(ulong userId, ulong guildId, CancellationToken token = default);
    Task SaveCharacterAsync(Character character, CancellationToken token = default);
    Task DeleteCharacterAsync(ulong charId, CancellationToken cancellationToken);
}