using DiscordRPG.Application.Models;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetAllActivitiesQuery : IQuery<IEnumerable<ActivityReadModel>>
{
}

public class GetAllActivitiesQueryHandler : IQueryHandler<GetAllActivitiesQuery, IEnumerable<ActivityReadModel>>
{
    private readonly IMongoDbReadModelStore<ActivityReadModel> store;

    public GetAllActivitiesQueryHandler(IMongoDbReadModelStore<ActivityReadModel> store)
    {
        this.store = store;
    }

    public async Task<IEnumerable<ActivityReadModel>> ExecuteQueryAsync(GetAllActivitiesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await store.FindAsync(r => true, cancellationToken: cancellationToken);
        return result.ToList(cancellationToken: cancellationToken);
    }
}