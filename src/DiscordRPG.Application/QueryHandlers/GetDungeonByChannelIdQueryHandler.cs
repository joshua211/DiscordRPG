using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetDungeonByChannelIdQueryHandler : QueryHandler<GetDungeonByChannelIdQuery, Dungeon>
{
    private readonly IRepository<Dungeon> repository;

    public GetDungeonByChannelIdQueryHandler(IRepository<Dungeon> repository)
    {
        this.repository = repository;
    }

    public override async Task<Dungeon> Handle(GetDungeonByChannelIdQuery request,
        CancellationToken cancellationToken = default)
    {
        return (await repository.FindAsync(d => d.DungeonChannelId == request.ChannelId, cancellationToken))
            .FirstOrDefault()!;
    }
}