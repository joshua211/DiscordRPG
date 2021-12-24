using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetAllCharactersInGuildQueryHandler : QueryHandler<GetAllCharactersInGuildQuery, IEnumerable<Character>>
{
    private readonly IRepository<Character> repository;

    public GetAllCharactersInGuildQueryHandler(ILogger logger, IRepository<Character> repository) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<IEnumerable<Character>> Handle(GetAllCharactersInGuildQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        var result = await repository.FindAsync(c => c.GuildId.Value == request.GuildId.Value, cancellationToken);

        return result;
    }
}