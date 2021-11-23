using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Core.ValueObjects;
using Serilog;
using ActivityType = DiscordRPG.Core.Enums.ActivityType;

namespace DiscordRPG.Client.Commands;

public class EnterDungeon : DialogCommandBase<EnterDungeonDialog>
{
    private readonly IActivityService activityService;
    private readonly ICharacterService characterService;

    public EnterDungeon(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService) : base(client,
        logger)
    {
        this.activityService = activityService;
        this.characterService = characterService;
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
        var result = await characterService.GetCharacterAsync(user.Id, user.Guild.Id);
        if (!result.WasSuccessful)
        {
            EndDialog(dialog.UserId);
            await command.RespondAsync("Please create a character first!");
        }

        dialog.DungeonId = command.Channel.Id;
        dialog.CharId = result.Value.ID;

        var component = new ComponentBuilder()
            .WithButton("Enter Dungeon", CommandName + ".enter")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        var text = "Some flavour text and summary about the dungeon";

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
        EndDialog(dialog.DungeonId);

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Maybe another time!";
            properties.Components = null;
        });
    }

    private async Task HandleEnterDungeon(SocketMessageComponent component, EnterDungeonDialog dialog)
    {
        //get dungeon
        await activityService.QueueActivityAsync(dialog.CharId, TimeSpan.FromSeconds(15),
            ActivityType.Dungeon, new ActivityData(),
            CancellationToken.None);

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "Some more text about the dungeon";
        });

        EndDialog(dialog.DungeonId);
    }
}