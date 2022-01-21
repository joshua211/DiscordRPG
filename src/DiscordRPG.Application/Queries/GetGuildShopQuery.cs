using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;

namespace DiscordRPG.Application.Queries;

public class GetGuildShopQuery : IQuery<ShopReadModel>
{
    public GetGuildShopQuery(GuildId guildId)
    {
        GuildId = guildId;
    }

    public GuildId GuildId { get; private set; }
}

public class GetGuildShopQueryHandler : IQueryHandler<GetGuildShopQuery, ShopReadModel>
{
    private readonly IMongoDbReadModelStore<ShopReadModel> store;

    public GetGuildShopQueryHandler(IMongoDbReadModelStore<ShopReadModel> store)
    {
        this.store = store;
    }

    public async Task<ShopReadModel> ExecuteQueryAsync(GetGuildShopQuery query, CancellationToken cancellationToken)
    {
        var result = await store.FindAsync(s => s.GuildId == query.GuildId.Value, cancellationToken: cancellationToken);
        return result.FirstOrDefault(cancellationToken: cancellationToken);
    }
}