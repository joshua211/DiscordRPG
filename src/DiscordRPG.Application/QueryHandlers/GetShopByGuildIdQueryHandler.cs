using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetShopByGuildIdQueryHandler : QueryHandler<GetShopByGuildIdQuery, Shop>
{
    private readonly IRepository<Shop> repository;

    public GetShopByGuildIdQueryHandler(ILogger logger, IRepository<Shop> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Shop> Handle(GetShopByGuildIdQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.FindAsync(s => s.GuildId.Value == request.GuildId.Value, cancellationToken);

        return result.FirstOrDefault();
    }
}