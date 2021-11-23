using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;
using Serilog;

namespace DiscordRPG.Application.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly IMongoCollection<Guild> guilds;
    private readonly ILogger logger;

    public GuildRepository(IGuildDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger;
        var client = new MongoClient(databaseSettings.ConnectionString);
        guilds = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Guild>(databaseSettings.CollectionName);
    }

    public async Task<Guild> GetGuildAsync(ulong guildId, CancellationToken token)
    {
        logger.Verbose("Getting Guild {Id}", guildId);
        var result = await guilds.FindAsync(g => g.ServerId == guildId, cancellationToken: token);

        var guild = await result.FirstOrDefaultAsync(cancellationToken: token);

        logger.Verbose("Found guild: {@Guild}", guild);
        return guild;
    }

    public async Task SaveGuildAsync(Guild guild, CancellationToken cancellationToken)
    {
        logger.Verbose("Saving guild {@guild}", guild);
        await guilds.InsertOneAsync(guild, null, cancellationToken);
    }

    public async Task UpdateGuildAsync(Guild guild, CancellationToken cancellationToken)
    {
        logger.Verbose("Updating guild {@guild}");
        var result = await guilds.ReplaceOneAsync(g => g.ServerId == guild.ServerId, guild,
            cancellationToken: cancellationToken);
        logger.Verbose("Updated {Count} guilds", result.ModifiedCount);
    }

    public async Task DeleteGuildAsync(ulong guildId, CancellationToken cancellationToken)
    {
        logger.Verbose("Deleting guild {Dd}", guildId);
        var result = await guilds.DeleteOneAsync(g => g.ServerId == guildId, cancellationToken);
        logger.Verbose("Deleted {Count} guilds", result.DeletedCount);
    }
}