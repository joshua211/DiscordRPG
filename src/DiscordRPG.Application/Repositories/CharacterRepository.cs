using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly IMongoCollection<Character> characters;
    private readonly ILogger logger;

    public CharacterRepository(ICharacterDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        characters = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Character>(databaseSettings.CollectionName);
    }

    public async Task<Character> GetGuildCharacterAsync(ulong userId, ulong guildId, CancellationToken token = default)
    {
        logger.Verbose("Getting character {ChraId} for guild {guildId}", userId, guildId);
        var result =
            await characters.FindAsync(c => c.UserId == userId && c.GuildId == guildId, cancellationToken: token);

        var character = await result.FirstOrDefaultAsync(token);

        logger.Verbose("Found character: {@Char}", character);
        return character;
    }

    public async Task SaveCharacterAsync(Character character, CancellationToken token = default)
    {
        logger.Verbose("Saving character {@Char}", character);
        await characters.InsertOneAsync(character, cancellationToken: token);
    }

    public async Task DeleteCharacterAsync(ulong charId, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting character {CharId}", charId);
        var result = await characters.DeleteOneAsync(c => c.UserId == charId, cancellationToken: cancellationToken);
        logger.Verbose("Deleted {Count} character", result.DeletedCount);
    }
}