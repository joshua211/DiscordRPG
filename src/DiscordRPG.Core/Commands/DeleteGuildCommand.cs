namespace DiscordRPG.Core.Commands;

public class DeleteGuildCommand : Command
{
    public DeleteGuildCommand(ulong id)
    {
        Id = id;
    }

    public ulong Id { get; private set; }
}