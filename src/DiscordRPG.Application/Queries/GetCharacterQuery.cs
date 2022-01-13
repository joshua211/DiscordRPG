using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;

namespace DiscordRPG.Application.Queries;

public class GetCharacterQuery : IQuery<CharacterReadModel>
{
    public GetCharacterQuery(CharacterId characterId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}

public class GetCharacterQueryHandler : IQueryHandler<GetCharacterQuery, CharacterReadModel>
{
    private readonly IMongoDbReadModelStore<CharacterReadModel> store;

    public GetCharacterQueryHandler(IMongoDbReadModelStore<CharacterReadModel> store)
    {
        this.store = store;
    }

    public async Task<CharacterReadModel> ExecuteQueryAsync(GetCharacterQuery query,
        CancellationToken cancellationToken)
    {
        var id = query.CharacterId.Value;
        var result = await store.GetAsync(id, cancellationToken);

        return result.ReadModel;
    }
}