using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetDungeonByChannelIdQueryHandler : QueryHandler<GetDungeonByChannelIdQuery, Dungeon>
{
    private readonly IRepository<Dungeon> repository;

    public GetDungeonByChannelIdQueryHandler(IRepository<Dungeon> repository, ILogger logger) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Dungeon> Handle(GetDungeonByChannelIdQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return (await repository.FindAsync(d => d.DungeonChannelId.Value == request.ChannelId.Value, cancellationToken))
            .FirstOrDefault()!;
    }
}