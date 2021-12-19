﻿using System.Linq.Expressions;
using DiscordRPG.Application.Settings;
using DiscordRPG.Common;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class GuildRepository : IRepository<Guild>
{
    private readonly IMongoCollection<Guild> guilds;
    private readonly ILogger logger;

    public GuildRepository(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        guilds = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Guild>(databaseSettings.GuildCollectionName);
    }

    public async Task SaveAsync(Guild entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Saving guild {@guild}", entity);
        await guilds.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task UpdateAsync(Guild entity, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Updating guild {@guild}", entity);
        entity.LastModified = DateTime.UtcNow;
        var result = await guilds.ReplaceOneAsync(g => g.ID.Value == entity.ID.Value, entity,
            cancellationToken: cancellationToken);
        logger.Here().Verbose("Updated {Count} guilds", result.ModifiedCount);
    }

    public async Task DeleteAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Deleting guild {Dd}", id);
        var result = await guilds.DeleteOneAsync(g => g.ID.Value == id.Value, cancellationToken);
        logger.Here().Verbose("Deleted {Count} guilds", result.DeletedCount);
    }

    public async Task<Guild> GetAsync(Identity id, CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting Guild {Id}", id);
        var result = await guilds.FindAsync(g => g.ID.Value == id.Value, cancellationToken: cancellationToken);
        var guild = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        logger.Here().Verbose("Found guild: {@Guild}", guild);

        return guild;
    }

    public async Task<IEnumerable<Guild>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Getting all guilds");
        var result = await guilds.FindAsync(g => true);
        var list = await result.ToListAsync(cancellationToken);
        logger.Here().Verbose("Found {Count} guilds", list.Count);

        return list;
    }

    public async Task<IEnumerable<Guild>> FindAsync(Expression<Func<Guild, bool>> expression,
        CancellationToken cancellationToken)
    {
        logger.Here().Verbose("Finding dungeons with expression {Expression}", expression.ToString());
        var cursor = await guilds.FindAsync(expression, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken);
        logger.Here().Verbose("Found {Count} guilds", list.Count);

        return list;
    }
}