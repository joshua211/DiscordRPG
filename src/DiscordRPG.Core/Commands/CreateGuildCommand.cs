using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands;

public class CreateGuildCommand : Command
{
    public CreateGuildCommand(Guild guild)
    {
        Guild = guild;
    }

    public Guild Guild { get; private set; }
}