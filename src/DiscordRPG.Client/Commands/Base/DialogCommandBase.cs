﻿using Discord.WebSocket;
using DiscordRPG.Client.Dialogs;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class DialogCommandBase<T> : CommandBase where T : Dialog, new()
{
    private readonly Dictionary<ulong, T> dialogs;

    protected DialogCommandBase(DiscordSocketClient client, ILogger logger) : base(client, logger)
    {
        dialogs = new Dictionary<ulong, T>();
    }

    public override async Task HandleAsync(SocketSlashCommand command)
    {
        if (!await ShouldExecuteAsync(command))
            return;

        var dialog = (T) Activator.CreateInstance(typeof(T), command.User.Id)!;
        dialogs[dialog.UserId] = dialog;
        logger.Debug("Initiated new dialog: {@Dialog}", dialog);
        await Handle(command, dialog);
    }

    protected override async Task HandleSelection(SocketMessageComponent component, string id)
    {
        var dialog = dialogs[component.User.Id];
        await HandleSelection(component, id, dialog);
    }

    protected override async Task HandleButton(SocketMessageComponent component, string id)
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

    protected abstract Task Handle(SocketSlashCommand command, T dialog);
}