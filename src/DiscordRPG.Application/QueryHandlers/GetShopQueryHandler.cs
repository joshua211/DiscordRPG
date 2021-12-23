using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetShopQueryHandler : QueryHandler<GetShopQuery, Shop>
{
    private readonly IRepository<Shop> repository;

    public GetShopQueryHandler(ILogger logger, IRepository<Shop> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Shop> Handle(GetShopQuery request, CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return await repository.GetAsync(request.ShopId, cancellationToken);
    }
}