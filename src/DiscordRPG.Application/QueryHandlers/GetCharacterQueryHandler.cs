using DiscordRPG.Application.Queries;
using DiscordRPG.Common;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterQueryHandler : QueryHandler<GetCharacterQuery, Character>
{
    private readonly IRepository<Character> characterRepository;

    public GetCharacterQueryHandler(ILogger logger, IRepository<Character> characterRepository) : base(logger)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task<Character> Handle(GetCharacterQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Handling Query {Query}", request.GetType().Name);
        return await characterRepository.GetAsync(request.CharId, cancellationToken);
    }
}