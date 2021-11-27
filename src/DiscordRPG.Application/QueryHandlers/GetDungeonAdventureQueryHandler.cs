using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetDungeonAdventureQueryHandler : QueryHandler<GetDungeonAdventureQuery, DungeonResult>
{
    public GetDungeonAdventureQueryHandler(ILogger logger) : base(logger)
    {
    }

    public override Task<DungeonResult> Handle(GetDungeonAdventureQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return Task.FromResult(new DungeonResult(new List<Wound>() {new Wound("Cut to the dick", 1)}));
    }
}