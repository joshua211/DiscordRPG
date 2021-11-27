using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterActivityQueryHandler : QueryHandler<GetCharacterActivityQuery, Activity>
{
    private readonly IRepository<Activity> repository;

    public GetCharacterActivityQueryHandler(IRepository<Activity> repository, ILogger logger) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Activity> Handle(GetCharacterActivityQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.FindAsync(a => a.CharId == request.CharId, cancellationToken);

        return result.FirstOrDefault();
    }
}