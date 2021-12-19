using Discord;
using Discord.Net;
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

[RequireChannelName(ServerHandler.DungeonHallName)]
[RequireCharacter]
[RequireGuild]
[RequireNoCurrentActivity]
public class SearchDungeon : DialogCommandBase<SearchDungeonDialog>
{
    public SearchDungeon(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "search";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder = GetActivityDurationBuilder("Choose how long you are going to search");

            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Search for a new dungeon")
                .AddOption(optionBuilder);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (HttpException e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        SearchDungeonDialog dialog)
    {
        var user = command.User as SocketGuildUser;
        var character = context.Character;
        dialog.Character = character;
        dialog.ServerId = user.Guild.Id;

        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        var component = new ComponentBuilder()
            .WithButton("Search", CommandName + ".accept")
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary)
            .Build();

        //TODO some flavour text
        var text = $"This will take you {(int) duration} minutes, are you sure you want to search?";
        await command.RespondAsync(text, component: component, ephemeral: true);
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
        var result = await activityService.QueueActivityAsync(dialog.Character.ID, dialog.Duration,
            ActivityType.SearchDungeon, new ActivityData
            {
                PlayerLevel = dialog.Character.Level.CurrentLevel,
                ServerId = dialog.ServerId
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
            properties.Content = $"You've started searching, come back in {(int) dialog.Duration} minutes!";
        });
        EndDialog(dialog.UserId);
    }
}