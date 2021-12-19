using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using Serilog;
using ActivityType = DiscordRPG.Core.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireGuild]
[RequireActivity]
public class ShowActivity : DialogCommandBase<ShowActivityDialog>
{
    public ShowActivity(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "activity";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Check your current activity")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        ShowActivityDialog dialog)
    {
        dialog.Activity = context.Activity;
        dialog.Character = context.Character;


        var embed = GetActivityAsEmbed(dialog.Activity);

        var component = new ComponentBuilder()
            .WithButton("Stop activity", CommandName + ".stop", ButtonStyle.Danger)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        await command.RespondAsync(embed: embed, component: component, ephemeral: true);
    }

    private static Embed GetActivityAsEmbed(Activity dialogActivity)
    {
        //TODO fix on production
        var timeLeft = dialogActivity.StartTime +
                       TimeSpan.FromMinutes((int) dialogActivity.Duration) -
                       DateTime.UtcNow;

        var title = dialogActivity.Type switch
        {
            ActivityType.Dungeon => "Exploring a dungeon",
            ActivityType.Rest => "Resting",
            ActivityType.SearchDungeon => "Searching for a dungeon",
            _ => "???"
        };

        return new EmbedBuilder()
            .WithTitle(title)
            .WithDescription($"You are currently {title}")
            .AddField("Start Time", dialogActivity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
            .AddField("Minutes left", timeLeft.TotalMinutes).Build();
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, ShowActivityDialog dialog)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, ShowActivityDialog dialog) =>
        id switch
        {
            "cancel" => HandleCancel(component, dialog),
            "stop" => HandleStop(component, dialog),
            "accept-prompt" => HandleAcceptPrompt(component, dialog)
        };

    private async Task HandleAcceptPrompt(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        await activityService.StopActivityAsync(dialog.Activity);
        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "You stopped your adventure!";
        });
    }

    private async Task HandleStop(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        var msgComponent = new ComponentBuilder()
            .WithButton("Stop activity", CommandName + ".accept-prompt", ButtonStyle.Danger)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        await component.UpdateAsync(properties =>
        {
            properties.Embed = null;
            properties.Content = "If you stop now, you wont get any rewards!";
            properties.Components = msgComponent;
        });
    }

    private async Task HandleCancel(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        EndDialog(dialog.UserId);
        await component.UpdateAsync(properties => { properties.Components = null; });
    }
}