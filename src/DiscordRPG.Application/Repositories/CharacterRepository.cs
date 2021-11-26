using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class CharacterRepository : IRepository<Character>
{
    private readonly IMongoCollection<Character> characters;
    private readonly ILogger logger;

    public CharacterRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        characters = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Character>(databaseSettings.CharacterCollectionName);
    }

    public async Task SaveAsync(Character entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving character {@Character}", entity);
        await characters.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Character entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Updating Character {@Character}", entity);
        var result = await characters.ReplaceOneAsync(g => g.ID == entity.ID, entity,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} Characters", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting Character {Dd}", id);
        var result = await characters.DeleteOneAsync(c => c.ID == id, cancellationToken);
        logger.Verbose("Deleted {Count} characters", result.DeletedCount);
    }

    public async Task<Character> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Getting Character {Id}", id);
        var result = await characters.FindAsync(g => g.ID == id, cancellationToken: cancellationToken);

        var entity = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        logger.Verbose("Found Character: {@Character}", entity);
        return entity;
    }

    public async Task<IEnumerable<Character>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Verbose("Getting all Characters");

        var result = await characters.FindAsync(c => true, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Character>> FindAsync(Expression<Func<Character, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding Characters with expression {Expression}", expression);
        var cursor = await characters.FindAsync(expression, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }
}