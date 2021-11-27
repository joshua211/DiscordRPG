using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetGuildQueryHandler : QueryHandler<GetGuildQuery, Guild>
{
    private readonly IRepository<Guild> guildRepository;

    public GetGuildQueryHandler(IRepository<Guild> guildRepository, ILogger logger) : base(logger)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<Guild> Handle(GetGuildQuery request, CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return await guildRepository.GetAsync(request.Identity, cancellationToken);
    }
}