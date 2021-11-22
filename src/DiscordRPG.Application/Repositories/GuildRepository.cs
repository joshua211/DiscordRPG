using DiscordRPG.Application.Settings;
using DiscordRPG.Core.Repositories;
using MongoDB.Driver;

namespace DiscordRPG.Application.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly IMongoCollection<Guild> guilds;

    public GuildRepository(IGuildDatabaseSettings databaseSettings)
    {
        var client = new MongoClient(databaseSettings.ConnectionString);
        guilds = client.GetDatabase(databaseSettings.DatabaseName)
            .GetCollection<Guild>(databaseSettings.CollectionName);
    }

    public async Task<Guild> GetGuildAsync(ulong guildId, CancellationToken token)
    {
        var result = await guilds.FindAsync(g => g.ServerId == guildId, cancellationToken: token);

        return await result.FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task SaveGuildAsync(Guild guild, CancellationToken cancellationToken)
    {
        await guilds.InsertOneAsync(guild, null, cancellationToken);
    }

    public async Task UpdateGuildAsync(Guild guild, CancellationToken cancellationToken)
    {
        await guilds.ReplaceOneAsync(g => g.ServerId == guild.ServerId, guild, cancellationToken: cancellationToken);
    }

    public async Task DeleteGuildAsync(ulong guildId, CancellationToken cancellationToken)
    {
        await guilds.DeleteOneAsync(g => g.ServerId == guildId, cancellationToken);
    }
}