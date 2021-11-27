using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetActivityQueryHandler : QueryHandler<GetActivityQuery, Activity>
{
    private readonly IRepository<Activity> repository;

    public GetActivityQueryHandler(IRepository<Activity> repository, ILogger logger) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Activity> Handle(GetActivityQuery request, CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return await repository.GetAsync(request.Id, cancellationToken);
    }
}