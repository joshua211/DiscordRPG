using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetActivityQueryHandler : QueryHandler<GetActivityQuery, Activity>
{
    private readonly IRepository<Activity> repository;

    public GetActivityQueryHandler(IRepository<Activity> repository)
    {
        this.repository = repository;
    }

    public override async Task<Activity> Handle(GetActivityQuery request, CancellationToken cancellationToken = default)
    {
        return await repository.GetAsync(request.Id, cancellationToken);
    }
}