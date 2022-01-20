using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetAllDungeonsQuery : IQuery<IEnumerable<DungeonReadModel>>
{
    public GetAllDungeonsQuery(GuildId guildId)
    {
        GuildId = guildId;
    }

    public GuildId GuildId { get; private set; }
}

public class GetAllDungeonsQueryHandler : IQueryHandler<GetAllDungeonsQuery, IEnumerable<DungeonReadModel>>
{
    private readonly IMongoDbReadModelStore<DungeonReadModel> store;

    public GetAllDungeonsQueryHandler(IMongoDbReadModelStore<DungeonReadModel> store)
    {
        this.store = store;
    }

    public async Task<IEnumerable<DungeonReadModel>> ExecuteQueryAsync(GetAllDungeonsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await store.FindAsync(d => d.GuildId == query.GuildId.Value, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken: cancellationToken);
    }
}