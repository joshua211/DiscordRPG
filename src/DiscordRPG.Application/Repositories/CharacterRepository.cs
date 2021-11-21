using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly IMongoCollection<Character> characters;

    public CharacterRepository(ICharacterDatabaseSettings databaseSettings)
    {
        var client = new MongoClient(databaseSettings.ConnectionString);
        characters = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Character>(databaseSettings.CollectionName);
    }

    public async Task<Character> GetGuildCharacterAsync(ulong userId, ulong guildId, CancellationToken token = default)
    {
        var result =
            await characters.FindAsync(c => c.UserId == userId && c.GuildId == guildId, cancellationToken: token);
        return await result.FirstOrDefaultAsync(token);
    }

    public async Task SaveCharacterAsync(Character character, CancellationToken token = default)
    {
        await characters.InsertOneAsync(character, cancellationToken: token);
    }
}