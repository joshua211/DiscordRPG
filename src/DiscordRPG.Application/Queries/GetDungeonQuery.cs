using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Dungeon;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;

namespace DiscordRPG.Application.Queries;

public class GetDungeonQuery : IQuery<DungeonReadModel>
{
    public GetDungeonQuery(DungeonId dungeonId)
    {
        DungeonId = dungeonId;
    }

    public DungeonId DungeonId { get; private set; }
}

public class GetDungeonQueryHandler : IQueryHandler<GetDungeonQuery, DungeonReadModel>
{
    private readonly IMongoDbReadModelStore<DungeonReadModel> store;

    public GetDungeonQueryHandler(IMongoDbReadModelStore<DungeonReadModel> store)
    {
        this.store = store;
    }

    public async Task<DungeonReadModel> ExecuteQueryAsync(GetDungeonQuery query, CancellationToken cancellationToken)
    {
        var id = query.DungeonId.Value;
        var result = await store.GetAsync(id, cancellationToken);

        return result.ReadModel;
    }
}