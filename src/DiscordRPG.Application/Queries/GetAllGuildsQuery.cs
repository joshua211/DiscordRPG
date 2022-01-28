using DiscordRPG.Application.Models;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetAllGuildsQuery : IQuery<IEnumerable<GuildReadModel>>
{
}

public class GetAllGuildsQueryHandler : IQueryHandler<GetAllGuildsQuery, IEnumerable<GuildReadModel>>
{
    private readonly IMongoDbReadModelStore<GuildReadModel> store;

    public GetAllGuildsQueryHandler(IMongoDbReadModelStore<GuildReadModel> store)
    {
        this.store = store;
    }

    public async Task<IEnumerable<GuildReadModel>> ExecuteQueryAsync(GetAllGuildsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await store.FindAsync(r => true, cancellationToken: cancellationToken);
        return result.ToList(cancellationToken: cancellationToken);
    }
}