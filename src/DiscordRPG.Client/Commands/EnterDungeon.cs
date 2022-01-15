using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Activity.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using Serilog;

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
        dialog.CharId = context.Character!.Id;
        dialog.Dungeon = context.Dungeon!;
        dialog.GuildId = context.Guild.Id;
        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        if (context.Dungeon.Explorations.Value <= 0)
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
            $"Do you want to explore the level {context.Dungeon!.Level.Value} Dungeon {context.Dungeon!.Name}? This will take you {(int) duration} minutes";

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    [Handler("enter")]
    public async Task HandleEnterDungeon(SocketMessageComponent component, EnterDungeonDialog dialog)
    {
        var result = await activityService.QueueActivityAsync(new GuildId(dialog.GuildId),
            new CharacterId(dialog.CharId), dialog.Duration, Domain.Entities.Activity.Enums.ActivityType.Dungeon,
            new ActivityData(0, null, null, new DungeonId(dialog.Dungeon.Id), null), dialog.Context);

        await dungeonService.DecreaseExplorationsAsync(new GuildId(dialog.GuildId), new DungeonId(dialog.Dungeon.Id),
            dialog.Context);

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