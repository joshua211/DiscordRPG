using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetCharacterActivityQuery : IQuery<ActivityReadModel>
{
    public GetCharacterActivityQuery(CharacterId characterId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}

public class GetCharacterActivityQueryHandler : IQueryHandler<GetCharacterActivityQuery, ActivityReadModel>
{
    private readonly IMongoDbReadModelStore<ActivityReadModel> store;

    public GetCharacterActivityQueryHandler(IMongoDbReadModelStore<ActivityReadModel> store)
    {
        this.store = store;
    }

    public async Task<ActivityReadModel> ExecuteQueryAsync(GetCharacterActivityQuery query,
        CancellationToken cancellationToken)
    {
        var id = query.CharacterId.Value;
        var result = await store.FindAsync(a => a.CharacterId == id, cancellationToken: cancellationToken);

        return result.FirstOrDefault(cancellationToken: cancellationToken);
    }
}