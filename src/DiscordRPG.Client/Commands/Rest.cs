using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using Serilog;
using ActivityType = DiscordRPG.Domain.Entities.Activity.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireNoCurrentActivity]
[RequireChannelName(ServerHandler.Inn)]
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
            var optionBuilder = OptionHelper.GetActivityDurationBuilder("How long do you want to rest?");
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
        dialog.CharId = context.Character.Id;
        dialog.GuildId = context.Guild.Id;
        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        var component = new ComponentBuilder()
            .WithButton("Rest", GetCommandId("rest"))
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to rest at an inn and recover from your wounds?";

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    [Handler("rest")]
    public async Task HandleRest(SocketMessageComponent component, RestDialog dialog)
    {
        var guildId = new GuildId(dialog.GuildId);
        var charId = new CharacterId(dialog.CharId);
        var result = await activityService.QueueActivityAsync(guildId, charId, dialog.Duration, ActivityType.Rest, null,
            dialog.Context, CancellationToken.None);

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
}