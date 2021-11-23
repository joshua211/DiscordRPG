using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Guilds;

public class CreateGuildCommand : Command
{
    public CreateGuildCommand(Guild guild)
    {
        Guild = guild;
    }

    public Guild Guild { get; private set; }
}