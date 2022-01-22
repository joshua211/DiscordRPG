using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity;
using Serilog;
using ActivityType = DiscordRPG.Domain.Entities.Activity.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireGuild]
[RequireActivity]
public class ShowActivity : DialogCommandBase<ShowActivityDialog>
{
    public ShowActivity(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
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
        dialog.GuildId = new GuildId(context.Guild.Id);

        var embed = await GetActivityAsEmbedAsync(dialog.Activity, dialog);

        var component = new ComponentBuilder()
            .WithButton("Stop activity", CommandName + ".stop", ButtonStyle.Danger)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        await command.RespondAsync(embed: embed, component: component, ephemeral: true);
    }

    private async Task<Embed> GetActivityAsEmbedAsync(ActivityReadModel activity, ShowActivityDialog dialog)
    {
        var timeLeft = activity.StartTime +
                       TimeSpan.FromMinutes((int) activity.Duration) -
                       DateTime.UtcNow;

        switch (activity.Type)
        {
            case ActivityType.Dungeon:
                var dungeon =
                    await dungeonService.GetDungeonAsync(activity.ActivityData.DungeonId, dialog.Context);
                var description = "You are currently exploring a dungeon!";
                if (dungeon.WasSuccessful)
                    description =
                        $"You are currently exploring the {dungeon.Value.Rarity.ToString()} dungeon {dungeon.Value.Name} (Lvl: {dungeon.Value.Level})";

                return new EmbedBuilder()
                    .WithTitle("Exploring a dungeon")
                    .WithDescription(description)
                    .AddField("Start Time", activity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            case ActivityType.Rest:
                return new EmbedBuilder()
                    .WithTitle("Resting")
                    .WithDescription("You are currently resting!")
                    .AddField("Start Time", activity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            case ActivityType.SearchDungeon:
                return new EmbedBuilder()
                    .WithTitle("Searching for a dungeon")
                    .WithDescription("You are currently searching for a dungeon")
                    .AddField("Start Time", activity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
            default:
                return new EmbedBuilder()
                    .WithTitle("???")
                    .WithDescription("???")
                    .AddField("Start Time", activity.StartTime.ToString("dd.MM.yyyy HH:mm:ss"))
                    .AddField("Minutes left", (int) timeLeft.TotalMinutes).Build();
        }
    }

    [Handler("accept-prompt")]
    public async Task HandleAcceptPrompt(SocketMessageComponent component, ShowActivityDialog dialog)
    {
        await activityService.StopActivityAsync(dialog.GuildId, new ActivityId(dialog.Activity.Id), dialog.Context);
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