using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetCharacterActivityQuery : Query<Activity>
{
    public GetCharacterActivityQuery(string charId)
    {
        CharId = charId;
    }

    public string CharId { get; private set; }
}