using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetAllDungeonsQueryHandler : QueryHandler<GetAllDungeonsQuery, IEnumerable<Dungeon>>
{
    private readonly IRepository<Dungeon> repository;

    public GetAllDungeonsQueryHandler(ILogger logger, IRepository<Dungeon> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<IEnumerable<Dungeon>> Handle(GetAllDungeonsQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.GetAllAsync(cancellationToken);

        return result;
    }
}