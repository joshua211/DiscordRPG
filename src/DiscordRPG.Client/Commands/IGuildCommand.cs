using Discord.WebSocket;

namespace DiscordRPG.Client.Commands;

public interface IGuildCommand
{
    string CommandName { get; }
    Task InstallAsync(SocketGuild guild);

    Task HandleAsync(SocketSlashCommand command);
}