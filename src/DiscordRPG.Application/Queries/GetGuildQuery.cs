using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;

namespace DiscordRPG.Application.Queries;

public class GetGuildQuery : IQuery<GuildReadModel>
{
    public GetGuildQuery(GuildId guildId)
    {
        GuildId = guildId;
    }

    public GuildId GuildId { get; private set; }
}

public class GetGuildQueryHandler : IQueryHandler<GetGuildQuery, GuildReadModel>
{
    private readonly IMongoDbReadModelStore<GuildReadModel> store;

    public GetGuildQueryHandler(IMongoDbReadModelStore<GuildReadModel> store)
    {
        this.store = store;
    }

    public async Task<GuildReadModel> ExecuteQueryAsync(GetGuildQuery query, CancellationToken cancellationToken)
    {
        var id = query.GuildId.Value;
        var result = await store.GetAsync(id, cancellationToken: cancellationToken);
        return result.ReadModel;
    }
}