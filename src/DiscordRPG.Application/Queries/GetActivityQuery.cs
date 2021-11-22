using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetActivityQuery : Query<Activity>
{
    public GetActivityQuery(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}