using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetGuildQuery : Query<Guild>
{
    public GetGuildQuery(Identity identity)
    {
        Identity = identity;
    }

    public Identity Identity { get; private set; }
}