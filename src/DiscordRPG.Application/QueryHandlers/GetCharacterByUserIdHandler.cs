using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Repositories;

namespace DiscordRPG.Application.QueryHandlers;

public class GetCharacterByUserIdHandler : QueryHandler<GetCharacterByUserIdQuery, Character>
{
    protected readonly ICharacterRepository repository;

    public GetCharacterByUserIdHandler(ICharacterRepository repository)
    {
        this.repository = repository;
    }

    public override async Task<Character> Handle(GetCharacterByUserIdQuery request,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetGuildCharacterAsync(request.UserId, request.GuildId, cancellationToken);
    }
}