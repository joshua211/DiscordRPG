using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Dialogs;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class DialogCommandBase<T> : CommandBase where T : Dialog
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
        var dialog = (T) Activator.CreateInstance(typeof(T), command.User.Id, context.Context)!;
        dialogs[dialog.UserId] = dialog;
        logger.Debug("Initiated new dialog: {@Dialog}", dialog);
        await HandleDialogAsync(command, context, dialog);
    }

    protected override async Task HandleSelectionAsync(SocketMessageComponent component, MethodInfo method)
    {
        if (!dialogs.TryGetValue(component.User.Id, out var dialog))
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Content = null;
                properties.Embeds = null;
                properties.Embed = new EmbedBuilder().WithTitle("This dialog has been closed!").Build();
            });

            return;
        }

        await (Task) method.Invoke(this, new object[] {component, dialog});
    }

    protected override async Task HandleButtonAsync(SocketMessageComponent component, MethodInfo method)
    {
        if (!dialogs.TryGetValue(component.User.Id, out var dialog))
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Content = null;
                properties.Embeds = null;
                properties.Embed = new EmbedBuilder().WithTitle("This dialog has been closed!").Build();
            });

            return;
        }

        await (Task) method.Invoke(this, new object[] {component, dialog});
    }

    protected void EndDialog(ulong id)
    {
        dialogs[id] = null;
        logger.Debug("Ended dialog {Id}", id);
    }

    [Handler("cancel")]
    public async Task HandleCancel(SocketMessageComponent component, Dialog dialog)
    {
        EndDialog(dialog.UserId);

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Maybe another time!";
            properties.Components = null;
            properties.Embed = null;
            properties.Embeds = null;
        });
    }

    [Handler("share")]
    public async Task ShareEmbed(SocketMessageComponent component, Dialog dialog)
    {
        if (dialog is not IShareableDialog shareableDialog)
            return;

        await component.RespondAsync($"Shared by {component.User.Username}", embed: shareableDialog.ShareableEmbed);
    }

    protected abstract Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context, T dialog);
}