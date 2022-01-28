using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetAllGuildCharacters : IQuery<IEnumerable<CharacterReadModel>>
{
    public GetAllGuildCharacters(GuildId guildId)
    {
        GuildId = guildId;
    }

    public GuildId GuildId { get; private set; }
}

public class GetAllGuildCharactersQueryHandler : IQueryHandler<GetAllGuildCharacters, IEnumerable<CharacterReadModel>>
{
    private readonly IMongoDbReadModelStore<CharacterReadModel> store;

    public GetAllGuildCharactersQueryHandler(IMongoDbReadModelStore<CharacterReadModel> store)
    {
        this.store = store;
    }

    public async Task<IEnumerable<CharacterReadModel>> ExecuteQueryAsync(GetAllGuildCharacters query,
        CancellationToken cancellationToken)
    {
        var result = await store.FindAsync(c => c.GuildId == query.GuildId, cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken: cancellationToken);
    }
}