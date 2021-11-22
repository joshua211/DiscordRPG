using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Repositories;

namespace DiscordRPG.Application.QueryHandlers;

public class GetGuildQueryHandler : QueryHandler<GetGuildQuery, Guild>
{
    private readonly IGuildRepository guildRepository;

    public GetGuildQueryHandler(IGuildRepository guildRepository)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<Guild> Handle(GetGuildQuery request, CancellationToken cancellationToken = default)
    {
        return await guildRepository.GetGuildAsync(request.GuildId, cancellationToken);
    }
}