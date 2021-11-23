using Discord.WebSocket;

namespace DiscordRPG.Client.Commands.Base;

public interface IGuildCommand
{
    string CommandName { get; }
    Task InstallAsync(SocketGuild guild);

    Task HandleAsync(SocketSlashCommand command);
}