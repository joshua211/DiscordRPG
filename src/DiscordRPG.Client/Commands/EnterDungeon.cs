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

//TODO require dungeon
[RequireCharacter]
public class EnterDungeon : DialogCommandBase<EnterDungeonDialog>
{
    private readonly IDungeonService dungeonService;

    public EnterDungeon(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService) : base(client,
        logger, activityService, characterService)
    {
        this.dungeonService = dungeonService;
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

    protected override async Task Handle(SocketSlashCommand command, EnterDungeonDialog dialog)
    {
        var user = command.User as SocketGuildUser;
        var charResult = await characterService.GetCharacterAsync(user.Id, user.Guild.Id);
        if (!charResult.WasSuccessful)
        {
            EndDialog(dialog.UserId);
            await command.RespondAsync("Please create a character first!");
        }

        //TODO check if this is a dungeon
        var dungeonResult = await dungeonService.GetDungeonAsync(command.Channel.Id);
        if (!dungeonResult.WasSuccessful)
        {
            await command.RespondAsync("This command can only be used in a dungeon!");
            EndDialog(dialog.UserId);

            return;
        }

        dialog.CharId = charResult.Value.ID;
        dialog.Dungeon = dungeonResult.Value;

        var component = new ComponentBuilder()
            .WithButton("Enter Dungeon", CommandName + ".enter")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to enter the level {dungeonResult.Value.DungeonLevel} Dungeon {dungeonResult.Value.Name}?";

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
                DungeonId = dialog.Dungeon.DungeonChannelId,
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