using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class GetCharacter : DialogCommandBase<ShowCharacterDialog>
{
    public GetCharacter(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "character";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Show your character")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        ShowCharacterDialog dialog)
    {
        var character = context.Character!;
        var embed = new EmbedBuilder()
            .WithTitle(character.CharacterName)
            .WithDescription("No Title equiped")
            .WithColor(Color.DarkGreen)
            .AddField("Money", $"{character.Money}$")
            .AddField("Race", character.CharacterRace.RaceName, true)
            .AddField("Class", character.CharacterClass.ClassName, true)
            .AddField("Level", character.Level.CurrentLevel, true)
            .AddField("Experience", $"{character.Level.CurrentExp}/{character.Level.RequiredExp}")
            .AddField("\u200B", "\u200B")
            .AddField("Health", $"{character.CurrentHealth}/{character.MaxHealth}", true)
            .AddField("Armor", character.Armor, true)
            .AddField("Magic Armor", character.MagicArmor, true)
            .AddField("Damage", $"{character.TotalDamage.Value} {character.TotalDamage.DamageType}")
            .AddField("Strength", character.Stength, true)
            .AddField("Agility", character.Agility, true)
            .AddField("\u200b", "\u200b", true)
            .AddField("Vitality", character.Vitality, true)
            .AddField("Intelligence", character.Intelligence, true)
            .AddField("\u200b", "\u200b", true)
            .Build();

        dialog.ShareableEmbed = embed;

        var menu = new ComponentBuilder().WithButton("Share", GetCommandId("share"), ButtonStyle.Primary)
            .WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary).Build();

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }
}