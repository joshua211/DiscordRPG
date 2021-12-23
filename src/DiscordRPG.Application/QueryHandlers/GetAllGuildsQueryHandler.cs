using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetAllGuildsQueryHandler : QueryHandler<GetAllGuildsQuery, IEnumerable<Guild>>
{
    private readonly IRepository<Guild> repository;

    public GetAllGuildsQueryHandler(ILogger logger, IRepository<Guild> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<IEnumerable<Guild>> Handle(GetAllGuildsQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.GetAllAsync(cancellationToken);

        return result;
    }
}