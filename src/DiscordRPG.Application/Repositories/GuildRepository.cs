using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class GuildRepository : IRepository<Guild>
{
    private readonly IMongoCollection<Guild> guilds;
    private readonly ILogger logger;

    public GuildRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        guilds = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Guild>(databaseSettings.GuildCollectionName);
    }

    public async Task SaveAsync(Guild entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving guild {@guild}", entity);
        await guilds.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Guild entity, CancellationToken cancellationToken)
    {
        logger.Verbose("Updating guild {@guild}", entity);
        var result = await guilds.ReplaceOneAsync(g => g.ID == entity.ID, entity,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} guilds", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting guild {Dd}", id);
        var result = await guilds.DeleteOneAsync(g => g.ID == id, cancellationToken);
        logger.Verbose("Deleted {Count} guilds", result.DeletedCount);
    }

    public async Task<Guild> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Verbose("Getting Guild {Id}", id);
        var result = await guilds.FindAsync(g => g.ID == id, cancellationToken: cancellationToken);

        var guild = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        logger.Verbose("Found guild: {@Guild}", guild);
        return guild;
    }

    public async Task<IEnumerable<Guild>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Verbose("Getting all guilds");

        var result = await guilds.FindAsync(g => true);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guild>> FindAsync(Expression<Func<Guild, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Verbose("Finding dungeons with expression {Expression}", expression.ToString());
        var cursor = await guilds.FindAsync(expression, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }
}