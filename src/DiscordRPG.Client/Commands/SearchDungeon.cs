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

public class SearchDungeon : DialogCommandBase<SearchDungeonDialog>
{
    private readonly IActivityService activityService;
    private readonly ICharacterService characterService;

    public SearchDungeon(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService) : base(client,
        logger)
    {
        this.activityService = activityService;
        this.characterService = characterService;
    }

    public override string CommandName => "search";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Search for a new dungeon");

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (ApplicationCommandException e)
        {
            logger.Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task Handle(SocketSlashCommand command, SearchDungeonDialog dialog)
    {
        var user = command.User as SocketGuildUser;
        var result = await characterService.GetCharacterAsync(dialog.UserId, user.Guild.Id);
        if (!result.WasSuccessful)
        {
            await command.RespondAsync("Please create a character first!");
            EndDialog(dialog.UserId);

            return;
        }

        var character = result.Value;
        dialog.CharId = character.ID;
        dialog.Level = character.Level.CurrentLevel;

        var activityResult = await activityService.GetCharacterActivityAsync(character.ID, CancellationToken.None);
        if (activityResult.WasSuccessful)
        {
            var timeLeft = ((activityResult.Value.StartTime + activityResult.Value.Duration) - DateTime.UtcNow);
            await command.RespondAsync($"You're already on an adventure, try again in {timeLeft.Minutes} minutes!");
            EndDialog(dialog.UserId);

            return;
        }


        var component = new ComponentBuilder()
            .WithButton("Search", CommandName + ".accept")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        //TODO some flavour text
        var text = "Do you want to search?";
        await command.RespondAsync(text, component: component);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, SearchDungeonDialog dialog)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, SearchDungeonDialog dialog) =>
        id switch
        {
            "accept" => HandleSearchDungeon(component, dialog),
            "cancel" => HandleCancelSearch(component, dialog),
            _ => ComponentIdNotHandled(id)
        };

    private async Task HandleCancelSearch(SocketMessageComponent component, SearchDungeonDialog dialog)
    {
        EndDialog(dialog.UserId);
        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "Maybe some other time!";
        });
    }

    private async Task HandleSearchDungeon(SocketMessageComponent component, SearchDungeonDialog dialog)
    {
        //TODO proper duration
        var result = await activityService.QueueActivityAsync(dialog.CharId, TimeSpan.FromSeconds(120),
            ActivityType.SearchDungeon, new ActivityData
            {
                PlayerLevel = dialog.Level
            });

        if (!result.WasSuccessful)
        {
            await component.RespondAsync(
                "Something went wrong, sorry!\nMaybe try again some other time and hope that this has been fixed :)");
            EndDialog(dialog.UserId);
        }

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "You started searching, cool!";
        });
        EndDialog(dialog.UserId);
    }
}