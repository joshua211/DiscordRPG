using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

public class CreateCharacter : DialogCommandBase<CreateCharacterDialog>
{
    private readonly ICharacterService characterService;

    public CreateCharacter(DiscordSocketClient client, ICharacterService characterService, ILogger logger) : base(
        client, logger)
    {
        this.characterService = characterService;
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

    protected override async Task Handle(SocketSlashCommand command, CreateCharacterDialog dialog)
    {
        var name = command.Data.Options.First().Value as string;
        dialog.Name = name;

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
        var @class = new Class(dialog.Class);
        var race = new Race(dialog.Race);
        var result = await characterService.CreateCharacterAsync(component.User.Id, dialog.Name, @class, race);
        if (!result.WasSuccessful)
        {
            await component.RespondAsync("Something went wrong! " + result.ErrorMessage);
            return;
        }

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = $"Welcome, {dialog.Name}!";
        });
    }
}