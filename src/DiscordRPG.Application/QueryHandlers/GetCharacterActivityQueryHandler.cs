using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterActivityQueryHandler : QueryHandler<GetCharacterActivityQuery, Activity>
{
    private readonly IRepository<Activity> repository;

    public GetCharacterActivityQueryHandler(IRepository<Activity> repository)
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