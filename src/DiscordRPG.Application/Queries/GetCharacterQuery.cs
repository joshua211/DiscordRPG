using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetCharacterQuery : Query<Character>
{
    public GetCharacterQuery(Identity charId)
    {
        CharId = charId;
    }

    public Identity CharId { get; private set; }
}