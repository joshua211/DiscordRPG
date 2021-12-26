using DiscordRPG.Core.Entities;
using DiscordRPG.DiagnosticConsole.Settings;
using MongoDB.Driver;

namespace DiscordRPG.DiagnosticConsole.Importers;

public class CharacterImporter : ICharacterImporter
{
    private readonly IMongoCollection<Character> collection;

    public CharacterImporter(IMongoClient client, IDatabaseSettings settings)
    {
        collection = client.GetDatabase(settings.LiveDatabaseName)
            .GetCollection<Character>(settings.CharacterCollectionName);
    }

    public async Task<IEnumerable<Character>> GetCharactersAsync()
    {
        return (await collection.FindAsync(c => true)).ToList();
    }

    public async Task<int> UpdateCharactersAsync(IEnumerable<Character> characters)
    {
        var bulk = new List<WriteModel<Character>>();
        foreach (var character in characters)
        {
            bulk.Add(new ReplaceOneModel<Character>(
                new ExpressionFilterDefinition<Character>(c => c.ID.Value == character.ID.Value), character));
        }

        var result = await collection.BulkWriteAsync(bulk);

        return (int) result.MatchedCount;
    }
}

public interface ICharacterImporter
{
    Task<IEnumerable<Character>> GetCharactersAsync();
    Task<int> UpdateCharactersAsync(IEnumerable<Character> characters);
}