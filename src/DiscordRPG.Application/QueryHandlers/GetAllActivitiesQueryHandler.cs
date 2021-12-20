using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetAllActivitiesQueryHandler : QueryHandler<GetAllActivitiesQuery, IEnumerable<Activity>>
{
    private readonly IRepository<Activity> repository;

    public GetAllActivitiesQueryHandler(ILogger logger, IRepository<Activity> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<IEnumerable<Activity>> Handle(GetAllActivitiesQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.GetAllAsync(cancellationToken);

        return result;
    }
}