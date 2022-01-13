using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Entities.Activity;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;

namespace DiscordRPG.Application.Queries;

public class GetActivityQuery : IQuery<ActivityReadModel>
{
    public GetActivityQuery(ActivityId activityId)
    {
        ActivityId = activityId;
    }

    public ActivityId ActivityId { get; private set; }
}

public class GetActivityQueryHandler : IQueryHandler<GetActivityQuery, ActivityReadModel>
{
    private readonly IMongoDbReadModelStore<ActivityReadModel> store;

    public GetActivityQueryHandler(IMongoDbReadModelStore<ActivityReadModel> store)
    {
        this.store = store;
    }

    public async Task<ActivityReadModel> ExecuteQueryAsync(GetActivityQuery query, CancellationToken cancellationToken)
    {
        var id = query.ActivityId.Value;
        var result = await store.GetAsync(id, cancellationToken: cancellationToken);
        return result.ReadModel;
    }
}