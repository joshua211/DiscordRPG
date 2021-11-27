using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterByUserIdHandler : QueryHandler<GetCharacterByUserIdQuery, Character>
{
    protected readonly IRepository<Character> repository;

    public GetCharacterByUserIdHandler(IRepository<Character> repository, ILogger logger) : base(logger)
    {
        this.repository = repository;
    }

    public override async Task<Character> Handle(GetCharacterByUserIdQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return (await repository.FindAsync(c => c.UserId.Value == request.UserId && c.GuildId.Value == request.GuildId,
            cancellationToken)).FirstOrDefault();
    }
}