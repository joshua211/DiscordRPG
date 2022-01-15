using System.Text;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
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
        var result = await characterService.GetCharacterAsync(new CharacterId(user.Id.ToString()), context.Context);
        if (result.Value != null)
        {
            await command.RespondAsync("You can only create one character on each server!");
            EndDialog(user.Id);
            return;
        }

        var name = command.Data.Options.First().Value as string;
        dialog.Name = name;
        dialog.GuildId = context.Guild.Id;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    private Embed GetDisplayEmbed(CreateCharacterDialog dialog)
    {
        if (dialog.Race is null)
        {
            return new EmbedBuilder().WithDescription($"Alright {dialog.Name}, choose a race first!").Build();
        }

        if (dialog.Class is null)
        {
            return new EmbedBuilder()
                .WithDescription($"A {dialog.Race.Name} huh? Anyway, choose your path next!")
                .Build();
        }

        var race = dialog.Race;
        var charClass = dialog.Class;
//race
        var builder = new EmbedBuilder()
            .WithTitle("Character Creation")
            .WithDescription("A summary of your choices. Change them as you wish")
            .AddField("Race", race.Name, true)
            .AddField("Description", race.Description);
        var strengths = Races.GetStrengths(race).ToList();
        var sb = new StringBuilder();
        for (var i = 0; i < strengths.Count; i++)
        {
            sb.Append(strengths[i]);
            if (i < strengths.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Strengths", sb.ToString());

        var weaknesses = Races.GetWeaknesses(race).ToList();
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
            .AddField("Class", charClass.Name, true)
            .AddField("Description", charClass.Description);
        strengths = Classes.GetStrengths(charClass).ToList();
        sb = new StringBuilder();
        for (var i = 0; i < strengths.Count; i++)
        {
            sb.Append(strengths[i]);
            if (i < strengths.Count - 1)
                sb.Append(", ");
        }

        builder.AddField("Strengths", sb.ToString());

        weaknesses = Classes.GetWeaknesses(charClass).ToList();
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
        if (dialog.Race is null)
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your race")
                .WithCustomId(GetCommandId("race-select"));

            foreach (var race in Races.GetAllRaces())
            {
                menuBuilder.AddOption(race.Name, race.Name, race.Description);
            }

            return new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
        }

        if (dialog.Class is null)
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Choose your class")
                .WithCustomId(GetCommandId("class-select"));
            foreach (var item in Classes.GetAllClasses())
            {
                menuBuilder.AddOption(item.Name, item.Name, item.Description);
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
        var raceName = component.Data.Values.FirstOrDefault();
        dialog.Race = Races.GetRace(raceName);

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
        var className = component.Data.Values.FirstOrDefault();
        dialog.Class = Classes.GetClass(className);

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
            await characterService.CreateCharacterAsync(new CharacterId(guildUser.Id.ToString()),
                new GuildId(dialog.GuildId), dialog.Name,
                dialog.Class,
                dialog.Race, dialog.Context);
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
        dialog.Class = null;
        dialog.Race = null;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }
}