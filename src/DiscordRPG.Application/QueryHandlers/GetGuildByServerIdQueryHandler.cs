using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetGuildByServerIdQueryHandler : QueryHandler<GetGuildByServerIdQuery, Guild>
{
    private readonly IRepository<Guild> guildRepository;

    public GetGuildByServerIdQueryHandler(IRepository<Guild> guildRepository, ILogger logger) : base(logger)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<Guild> Handle(GetGuildByServerIdQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return (await guildRepository.FindAsync(g => g.ServerId == request.GuildId, cancellationToken))
            .FirstOrDefault();
    }
}