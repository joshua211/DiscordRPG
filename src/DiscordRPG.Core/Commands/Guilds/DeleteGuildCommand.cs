namespace DiscordRPG.Core.Commands.Guilds;

public class DeleteGuildCommand : Command
{
    public DeleteGuildCommand(DiscordId id)
    {
        Id = id;
    }

    public DiscordId Id { get; private set; }
}