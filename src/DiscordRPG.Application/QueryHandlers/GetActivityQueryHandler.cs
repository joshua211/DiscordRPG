using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Repositories;

namespace DiscordRPG.Application.QueryHandlers;

public class GetActivityQueryHandler : QueryHandler<GetActivityQuery, Activity>
{
    private readonly IActivityRepository repository;

    public GetActivityQueryHandler(IActivityRepository repository)
    {
        this.repository = repository;
    }

    public override async Task<Activity> Handle(GetActivityQuery request, CancellationToken cancellationToken = default)
    {
        return await repository.GetActivityAsync(request.Id, cancellationToken);
    }
}