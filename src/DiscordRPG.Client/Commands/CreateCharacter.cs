using System.Text;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.DomainServices;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireChannelName(ServerHandler.GuildHallName)]
[RequireGuild]
public class CreateCharacter : DialogCommandBase<CreateCharacterDialog>
{
    private readonly IClassService classService;
    private readonly IRaceService raceService;

    public CreateCharacter(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IClassService classService, IRaceService raceService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
        this.classService = classService;
        this.raceService = raceService;
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
                    isRequired: true);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (HttpException e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        CreateCharacterDialog dialog)
    {
        var user = command.User as IGuildUser;
        var result = await characterService.GetUsersCharacterAsync(user.Id.ToString(), context.Guild.ID);
        if (result.WasSuccessful)
        {
            await command.RespondAsync("You can only create one character on each server!");
            EndDialog(user.Id);
            return;
        }

        var name = command.Data.Options.First().Value as string;
        dialog.Name = name;
        dialog.GuildId = context.Guild.ID;

        await ChooseRace(command, dialog);
    }

    private async Task ChooseRace(IDiscordInteraction interaction, CreateCharacterDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Choose your race")
            .WithCustomId(CommandName + ".race-select");

        foreach (var (id, race) in raceService.GetAllRaces())
        {
            menuBuilder.AddOption(race.RaceName, id.ToString(), race.Description);
        }

        var component = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
        var text = $"Alright {dialog.Name}, choose a race for yourself first!";
        if (interaction is SocketMessageComponent comp)
        {
            await comp.UpdateAsync(properties =>
            {
                properties.Content = text;
                properties.Embed = null;
                properties.Components = component;
            });
        }
        else
        {
            await interaction.RespondAsync(text, component: component,
                ephemeral: true);
        }
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
            "restart" => HandleRestart(component, dialog),
            "cancel" => HandleCancel(component, dialog),
            _ => throw new Exception("Cant handle " + id)
        };

    private Task HandleRaceSelect(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var raceId = int.Parse(component.Data.Values.FirstOrDefault());
        dialog.RaceId = raceId;

        return component.UpdateAsync(properties =>
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your class")
                .WithCustomId(CommandName + ".class-select");
            foreach (var item in classService.GetAllClasses())
            {
                var @class = item.@class;
                menuBuilder.AddOption(@class.ClassName, item.id.ToString(), @class.Description);
            }

            var component = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();

            var choosenRace = raceService.GetRace(raceId);

            properties.Components = component;
            properties.Content =
                $"{choosenRace.RaceName}, huh? Alright, its time to choose your path!";
        });
    }

    private Task HandleClassSelect(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var classId = int.Parse(component.Data.Values.FirstOrDefault());
        dialog.ClassId = classId;

        var race = raceService.GetRace(dialog.RaceId);
        var charClass = classService.GetClass(classId);
//race
        var builder = new EmbedBuilder()
            .WithTitle("Character Creation")
            .WithDescription("A summary of your choices. Change them as you wish")
            .AddField("Race", race.RaceName, true)
            .AddField("Description", race.Description);
        var strengths = raceService.GetStrengths(race).ToList();
        var sb = new StringBuilder();
        for (var i = 0; i < strengths.Count; i++)
        {
            sb.Append(strengths[i]);
            if (i < strengths.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Strengths", sb.ToString());

        var weaknesses = raceService.GetWeaknesses(race).ToList();
        sb = new StringBuilder();
        for (var i = 0; i < weaknesses.Count; i++)
        {
            sb.Append(weaknesses[i]);
            if (i < weaknesses.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Weaknesses", sb.ToString());
//class
        builder
            .AddField("\u200B", "\u200B")
            .AddField("Class", charClass.ClassName, true)
            .AddField("Description", charClass.Description);
        strengths = classService.GetStrengths(charClass).ToList();
        sb = new StringBuilder();
        for (var i = 0; i < strengths.Count; i++)
        {
            sb.Append(strengths[i]);
            if (i < strengths.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Strengths", sb.ToString());

        weaknesses = classService.GetWeaknesses(charClass).ToList();
        sb = new StringBuilder();
        for (var i = 0; i < weaknesses.Count; i++)
        {
            sb.Append(weaknesses[i]);
            if (i < weaknesses.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Weaknesses", sb.ToString());

        builder
            .AddField("Strength", charClass.BaseStrength, true)
            .AddField("Vitality", charClass.BaseVitality, true)
            .AddField("\u200B", "\u200B", true)
            .AddField("Agility", charClass.BaseAgility, true)
            .AddField("Intelligence", charClass.BaseIntelligence, true);

        return component.UpdateAsync(properties =>
        {
            var component = new ComponentBuilder()
                .WithButton("Create character", CommandName + ".submit")
                .WithButton("Let me start over", CommandName + ".restart", ButtonStyle.Secondary)
                .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Danger)
                .Build();

            properties.Components = component;
            properties.Content = string.Empty;
            properties.Embed = builder.Build();
        });
    }

    private async Task HandleSubmit(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var guildUser = component.User as SocketGuildUser;
        var result =
            await characterService.CreateCharacterAsync(guildUser.Id.ToString(), dialog.GuildId, dialog.Name,
                dialog.ClassId,
                dialog.RaceId);
        if (!result.WasSuccessful)
        {
            await component.RespondAsync("Something went wrong! " + result.ErrorMessage);
            EndDialog(component.User.Id);

            return;
        }

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Embed = null;
            properties.Content = $"Welcome, {dialog.Name}! You can view your character by typing `/character`";
        });
        EndDialog(component.User.Id);
    }

    private async Task HandleCancel(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        EndDialog(dialog.UserId);

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Maybe another time!";
            properties.Components = null;
            properties.Embed = null;
        });
    }

    private async Task HandleRestart(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        await ChooseRace(component, dialog);
    }
}