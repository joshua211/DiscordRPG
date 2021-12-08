using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using Serilog;
using ActivityType = DiscordRPG.Core.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireNoCurrentActivity]
[RequireChannelName(ServerHandler.GuildHallName)]
[RequireGuild]
public class Rest : DialogCommandBase<RestDialog>
{
    public Rest(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "rest";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder = GetActivityDurationBuilder("How long do you want to rest?");
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Rest and recover from your wounds")
                .AddOption(optionBuilder)
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        RestDialog dialog)
    {
        dialog.CharId = context.Character.ID;
        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        var component = new ComponentBuilder()
            .WithButton("Rest", CommandName + ".rest")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to rest at an inn and recover from your wounds?";

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, RestDialog dialog)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, RestDialog dialog) =>
        id switch
        {
            "rest" => HandleRest(component, dialog),
            "cancel" => HandleCancel(component, dialog),
            _ => ComponentIdNotHandled(id)
        };

    private async Task HandleRest(SocketMessageComponent component, RestDialog dialog)
    {
        var result = await activityService.QueueActivityAsync(dialog.CharId, dialog.Duration,
            ActivityType.Rest, new ActivityData());

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            if (result.WasSuccessful)
                properties.Content =
                    $"You are resting! Come back in {(int) dialog.Duration} minutes.";
            else
                properties.Content = $"Something went wrong, please try again in a few minutes!";
        });

        EndDialog(dialog.UserId);
    }

    private async Task HandleCancel(SocketMessageComponent component, RestDialog dialog)
    {
        EndDialog(dialog.UserId);

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Maybe another time!";
            properties.Components = null;
        });
    }
}