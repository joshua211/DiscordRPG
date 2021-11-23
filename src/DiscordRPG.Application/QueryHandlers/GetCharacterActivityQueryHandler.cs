using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Repositories;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterActivityQueryHandler : QueryHandler<GetCharacterActivityQuery, Activity>
{
    private readonly IActivityRepository repository;

    public GetCharacterActivityQueryHandler(IActivityRepository repository)
    {
        this.repository = repository;
    }

    public override async Task<Activity> Handle(GetCharacterActivityQuery request,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.FindAsync(a => a.CharId == request.CharId, cancellationToken);

        return result.FirstOrDefault();
    }
}