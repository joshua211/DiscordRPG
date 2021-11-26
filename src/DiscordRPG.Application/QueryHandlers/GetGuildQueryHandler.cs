using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetGuildQueryHandler : QueryHandler<GetGuildQuery, Guild>
{
    private readonly IRepository<Guild> guildRepository;

    public GetGuildQueryHandler(IRepository<Guild> guildRepository)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<Guild> Handle(GetGuildQuery request, CancellationToken cancellationToken = default)
    {
        return await guildRepository.GetAsync(request.Identity, cancellationToken);
    }
}