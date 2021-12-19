using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class CharacterRepository : IRepository<Character>
{
    private readonly IMongoCollection<Character> characters;
    private readonly IClassService classService;
    private readonly ILogger logger;
    private readonly IRaceService raceService;

    public CharacterRepository(IDatabaseSettings databaseSettings, ILogger logger, IClassService classService,
        IRaceService raceService)
    {
        this.classService = classService;
        this.raceService = raceService;
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        characters = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Character>(databaseSettings.CharacterCollectionName);
    }

    public async Task SaveAsync(Character entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Saving character {@Character}", entity);
        await characters.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Character entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Updating Character {@Character}", entity);
        entity.LastModified = DateTime.UtcNow;
        var result = await characters.ReplaceOneAsync(g => g.ID.Value == entity.ID.Value, entity,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} Characters", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Deleting Character {Dd}", id);
        var result = await characters.DeleteOneAsync(c => c.ID.Value == id.Value, cancellationToken);
        logger.Here().Verbose("Deleted {Count} characters", result.DeletedCount);
    }

    public async Task<Character> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting Character {Id}", id);
        var result = await characters.FindAsync(g => g.ID.Value == id.Value, cancellationToken: cancellationToken);

        var entity = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        entity.ClassService = classService;
        entity.RaceService = raceService;
        logger.Here().Verbose("Found Character: {@Character}", entity);

        return entity;
    }

    public async Task<IEnumerable<Character>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting all Characters");

        var result = await characters.FindAsync(c => true, cancellationToken: cancellationToken);
        var list = await result.ToListAsync(cancellationToken);
        list.ForEach(c =>
        {
            c.ClassService = classService;
            c.RaceService = raceService;
        });
        logger.Here().Verbose("Found {Count} character", list.Count);

        return list;
    }

    public async Task<IEnumerable<Character>> FindAsync(Expression<Func<Character, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Finding Characters with expression {Expression}", expression);
        var cursor = await characters.FindAsync(expression, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken: cancellationToken);
        list.ForEach(c =>
        {
            c.ClassService = classService;
            c.RaceService = raceService;
        });
        logger.Here().Verbose("Found {Count} character", list.Count);

        return list;
    }
}