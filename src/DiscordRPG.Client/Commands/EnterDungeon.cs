using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
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
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Enter and explore the current dungeon");

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (ApplicationCommandException e)
        {
            logger.Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        EnterDungeonDialog dialog)
    {
        dialog.CharId = context.Character!.ID;
        dialog.Dungeon = context.Dungeon!;

        var component = new ComponentBuilder()
            .WithButton("Enter Dungeon", CommandName + ".enter")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to enter the level {context.Dungeon!.DungeonLevel} Dungeon {context.Dungeon!.Name}?";

        await command.RespondAsync(text, component: component);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, EnterDungeonDialog dialog)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, EnterDungeonDialog dialog) =>
        id switch
        {
            "enter" => HandleEnterDungeon(component, dialog),
            "cancel" => HandleCancelDungeon(component, dialog),
            _ => ComponentIdNotHandled(id)
        };

    private async Task HandleCancelDungeon(SocketMessageComponent component, EnterDungeonDialog dialog)
    {
        EndDialog(dialog.UserId);

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Maybe another time!";
            properties.Components = null;
        });
    }

    private async Task HandleEnterDungeon(SocketMessageComponent component, EnterDungeonDialog dialog)
    {
        //TODO set proper duration
        var duration = TimeSpan.FromSeconds(15);
        var result = await activityService.QueueActivityAsync(dialog.CharId, duration,
            ActivityType.Dungeon, new ActivityData
            {
                ThreadId = dialog.Dungeon.DungeonChannelId,
            });

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            if (result.WasSuccessful)
                properties.Content =
                    $"You have entered {dialog.Dungeon.Name}! Come back in {duration.Minutes} minutes.";
            else
                properties.Content = $"Something went wrong, please try again in a few minutes!";
        });

        EndDialog(dialog.UserId);
    }
}