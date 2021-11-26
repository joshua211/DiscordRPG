using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireChannelName(ServerHandler.GuildHallName)]
[RequireGuild]
public class CreateCharacter : DialogCommandBase<CreateCharacterDialog>
{
    public CreateCharacter(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "create-character";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Create your very own character")
                .AddOption("name", ApplicationCommandOptionType.String, "Choose a name for your character",
                    required: true);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (ApplicationCommandException e)
        {
            logger.Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        CreateCharacterDialog dialog)
    {
        var user = command.User as IGuildUser;
        var result = await characterService.GetCharacterAsync(user.Id, context.Guild.ID);
        if (result.WasSuccessful)
        {
            await command.RespondAsync("You can only create one character on each server!");
            EndDialog(user.Id);
            return;
        }

        var name = command.Data.Options.First().Value as string;
        dialog.Name = name;
        dialog.GuildId = context.Guild.ID;

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Choose your race")
            .WithCustomId(CommandName + ".race-select")
            .AddOption("Human", "human", "The most common and boring race")
            .AddOption("Elf", "elf", "The generic fantasy race");
        var component = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();

        await command.RespondAsync($"Now {name}, choose your race", component: component);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id,
        CreateCharacterDialog dialog) => id switch
    {
        "race-select" => HandleRaceSelect(component, dialog),
        "class-select" => HandleClassSelect(component, dialog),
        _ => throw new Exception("Cant handle " + id)
    };

    protected override Task HandleButton(SocketMessageComponent component, string id, CreateCharacterDialog dialog) =>
        id switch
        {
            "submit" => HandleSubmit(component, dialog),
            _ => throw new Exception("Cant handle " + id)
        };

    private Task HandleRaceSelect(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var race = component.Data.Values.FirstOrDefault();
        dialog.Race = race;

        return component.UpdateAsync(properties =>
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your class")
                .WithCustomId(CommandName + ".class-select")
                .AddOption("Warrior", "warrior", "The brute force kinda guy")
                .AddOption("Wizard", "wizard", "You're a harry, Wizard!");
            var component = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();

            properties.Components = component;
            properties.Content = $"Wonderful {dialog.Name}, now choose a class!";
        });
    }

    private Task HandleClassSelect(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var className = component.Data.Values.FirstOrDefault();
        dialog.Class = className;

        return component.UpdateAsync(properties =>
        {
            var component = new ComponentBuilder().WithButton("Create character?", CommandName + ".submit").Build();

            properties.Components = component;
            properties.Content = "Character creation text";
        });
    }

    private async Task HandleSubmit(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        //TODO validate
        var guildUser = component.User as SocketGuildUser;
        var @class = new Class(dialog.Class);
        var race = new Race(dialog.Race);
        var result =
            await characterService.CreateCharacterAsync(guildUser.Id, dialog.GuildId, dialog.Name, @class, race);
        if (!result.WasSuccessful)
        {
            await component.RespondAsync("Something went wrong! " + result.ErrorMessage);
            EndDialog(component.User.Id);

            return;
        }

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = $"Welcome, {dialog.Name}!";
        });
        EndDialog(component.User.Id);
    }
}