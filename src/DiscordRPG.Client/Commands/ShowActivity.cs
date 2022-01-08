using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;
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


        var embed = await GetActivityAsEmbedAsync(dialog.Activity);

        var component = new ComponentBuilder()
            .WithButton("Stop activity", CommandName + ".stop", ButtonStyle.Danger)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        await command.RespondAsync(embed: embed, component: component, ephemeral: true);
    }

    private async Task<Embed> GetActivityAsEmbedAsync(Activity dialogActivity)
    {
        var timeLeft = dialogActivity.StartTime +
                       TimeSpan.FromMinutes((int) dialogActivity.Duration) -
                       DateTime.UtcNow;

        switch (dialogActivity.Type)
        {
            case ActivityType.Dungeon:
                var dungeon =
                    await dungeonService.GetDungeonFromChannelIdAsync(
                        new DiscordId(dialogActivity.Data.ThreadId.ToString()));
                var description = "You are currently explore a dungeon!";
                if (dungeon.WasSuccessful)
                    description =
                        $"You are currently exploring the {dungeon.Value.Rarity.ToString()} dungeon {dungeon.Value.Name} (Lvl: {dungeon.Value.DungeonLevel})";

                return new EmbedBuilder()
                    .WithTitle("Exploring a dungeon")
                    .WithDescription(description)
                    .AddField("Start Time", dialogActivity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            case ActivityType.Rest:
                return new EmbedBuilder()
                    .WithTitle("Resting")
                    .WithDescription("You are currently resting!")
                    .AddField("Start Time", dialogActivity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            case ActivityType.SearchDungeon:
                return new EmbedBuilder()
                    .WithTitle("Searching for a dungeon")
                    .WithDescription("You are currently searching for a dungeon")
                    .AddField("Start Time", dialogActivity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            default:
                return new EmbedBuilder()
                    .WithTitle("???")
                    .WithDescription("???")
                    .AddField("Start Time", dialogActivity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
        }
    }

    [Handler("accept-prompt")]
    public async Task HandleAcceptPrompt(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        await activityService.StopActivityAsync(dialog.Activity);
        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "You stopped your adventure!";
        });
    }

    [Handler("stop")]
    public async Task HandleStop(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        var msgComponent = new ComponentBuilder()
            .WithButton("Stop activity", GetCommandId("accept-prompt"), ButtonStyle.Danger)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        await component.UpdateAsync(properties =>
        {
            properties.Embed = null;
            properties.Content = "If you stop now, you wont get any rewards!";
            properties.Components = msgComponent;
        });
    }
}