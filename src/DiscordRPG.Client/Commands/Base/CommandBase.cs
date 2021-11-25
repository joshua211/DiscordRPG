using Discord.WebSocket;
using DiscordRPG.Client.Commands.Attributes;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class CommandBase : IGuildCommand
{
    protected readonly DiscordSocketClient client;
    protected readonly ILogger logger;

    protected CommandBase(DiscordSocketClient client, ILogger logger)
    {
        this.client = client;
        this.logger = logger;

        client.SelectMenuExecuted += HandleSelectionAsync;
        client.ButtonExecuted += HandleButtonAsync;
    }

    public abstract string CommandName { get; }

    public abstract Task InstallAsync(SocketGuild guild);

    public abstract Task HandleAsync(SocketSlashCommand command);

    public async Task<bool> ShouldExecuteAsync(SocketSlashCommand command)
    {
        var requireName =
            (RequireChannelNameAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireChannelNameAttribute));
        if (requireName == null)
            return true;

        if (command.Channel.Name == requireName.ChannelName)
            return true;

        await command.RespondAsync($"This command can only be used in the {requireName.ChannelName} channel!");
        return false;
    }

    protected virtual Task HandleSelection(SocketMessageComponent component, string id)
    {
        return Task.CompletedTask;
    }

    protected virtual Task HandleButton(SocketMessageComponent component, string id)
    {
        return Task.CompletedTask;
    }

    private async Task HandleSelectionAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleSelection(component, idSplit[1]);
    }

    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleButton(component, idSplit[1]);
    }
}