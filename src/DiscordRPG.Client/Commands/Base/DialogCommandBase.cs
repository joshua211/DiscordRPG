using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Dialogs;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class DialogCommandBase<T> : CommandBase where T : Dialog, new()
{
    private readonly Dictionary<ulong, T> dialogs;

    protected DialogCommandBase(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
        dialogs = new Dictionary<ulong, T>();
    }

    protected override async Task HandleAsync(SocketSlashCommand command, GuildCommandContext context)
    {
        var dialog = (T) Activator.CreateInstance(typeof(T), command.User.Id)!;
        dialogs[dialog.UserId] = dialog;
        logger.Debug("Initiated new dialog: {@Dialog}", dialog);
        await HandleDialogAsync(command, context, dialog);
    }


    protected override async Task HandleSelectionAsync(SocketMessageComponent component, string id)
    {
        var dialog = dialogs[component.User.Id];
        await HandleSelection(component, id, dialog);
    }

    protected override async Task HandleButtonAsync(SocketMessageComponent component, string id)
    {
        var dialog = dialogs[component.User.Id];
        await HandleButton(component, id, dialog);
    }

    protected void EndDialog(ulong id)
    {
        dialogs[id] = null;
        logger.Debug("Ended dialog {Id}", id);
    }

    protected Task ComponentIdNotHandled(string id)
    {
        logger.Warning("Command does not handle the component id {Id}", id);
        return Task.CompletedTask;
    }

    protected abstract Task HandleSelection(SocketMessageComponent component, string id, T dialog);

    protected abstract Task HandleButton(SocketMessageComponent component, string id, T dialog);

    protected abstract Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context, T dialog);
}