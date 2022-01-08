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

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    private Embed GetDisplayEmbed(CreateCharacterDialog dialog)
    {
        if (dialog.RaceId == 0)
        {
            return new EmbedBuilder().WithDescription($"Alright {dialog.Name}, choose a race first!").Build();
        }

        if (dialog.ClassId == 0)
        {
            return new EmbedBuilder()
                .WithDescription($"A {raceService.GetRace(dialog.RaceId).RaceName} huh? Anyway, choose your path next!")
                .Build();
        }

        var race = raceService.GetRace(dialog.RaceId);
        var charClass = classService.GetClass(dialog.ClassId);
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

        return builder.Build();
    }

    private MessageComponent GetMenu(CreateCharacterDialog dialog)
    {
        if (dialog.RaceId == 0)
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your race")
                .WithCustomId(GetCommandId("race-select"));

            foreach (var (id, race) in raceService.GetAllRaces())
            {
                menuBuilder.AddOption(race.RaceName, id.ToString(), race.Description);
            }

            return new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
        }

        if (dialog.ClassId == 0)
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your class")
                .WithCustomId(GetCommandId("class-select"));
            foreach (var item in classService.GetAllClasses())
            {
                var @class = item.@class;
                menuBuilder.AddOption(@class.ClassName, item.id.ToString(), @class.Description);
            }

            return new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
        }

        return new ComponentBuilder()
            .WithButton("Create character", GetCommandId("submit"))
            .WithButton("Let me start over", GetCommandId("restart"), ButtonStyle.Secondary)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Danger)
            .Build();
    }

    [Handler("race-select")]
    public async Task RaceSelectHandler(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var raceId = int.Parse(component.Data.Values.FirstOrDefault());
        dialog.RaceId = raceId;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }

    [Handler("class-select")]
    public async Task ClassSelectHandler(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        var classId = int.Parse(component.Data.Values.FirstOrDefault());
        dialog.ClassId = classId;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }

    [Handler("submit")]
    public async Task HandleSubmit(SocketMessageComponent component, CreateCharacterDialog dialog)
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

    [Handler("restart")]
    private async Task HandleRestart(SocketMessageComponent component, CreateCharacterDialog dialog)
    {
        dialog.ClassId = 0;
        dialog.RaceId = 0;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }
}