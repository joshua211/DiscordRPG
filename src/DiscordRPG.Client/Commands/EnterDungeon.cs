using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using Serilog;
using ActivityType = DiscordRPG.Core.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireNoCurrentActivity]
[RequireDungeon]
[RequireGuild]
public class EnterDungeon : DialogCommandBase<EnterDungeonDialog>
{
    public EnterDungeon(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "enter";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder =
                OptionHelper.GetActivityDurationBuilder("How long do you want to explore this dungeon?");
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Enter and explore the current dungeon")
                .AddOption(optionBuilder);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (HttpException e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        EnterDungeonDialog dialog)
    {
        dialog.CharId = context.Character!.ID;
        dialog.Dungeon = context.Dungeon!;
        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        if (context.Dungeon.ExplorationsLeft <= 0)
        {
            await command.RespondAsync("This dungeon has already been thoroughly searched!");
            EndDialog(dialog.UserId);
            return;
        }

        var component = new ComponentBuilder()
            .WithButton("Enter Dungeon", GetCommandId("enter"))
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to explore the level {context.Dungeon!.DungeonLevel} Dungeon {context.Dungeon!.Name}? This will take you {(int) duration} minutes";

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    [Handler("enter")]
    public async Task HandleEnterDungeon(SocketMessageComponent component, EnterDungeonDialog dialog)
    {
        var result = await activityService.QueueActivityAsync(dialog.CharId, dialog.Duration,
            ActivityType.Dungeon, new ActivityData
            {
                DungeonId = dialog.Dungeon.ID,
                ThreadId = dialog.Dungeon.DungeonChannelId,
            });

        await dungeonService.DecreaseExplorationsAsync(dialog.Dungeon);

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            if (result.WasSuccessful)
                properties.Content =
                    $"You have entered {dialog.Dungeon.Name}! Come back in {(int) dialog.Duration} minutes.";
            else
                properties.Content = $"Something went wrong, please try again in a few minutes!";
        });

        EndDialog(dialog.UserId);
    }
}