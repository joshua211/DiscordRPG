using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireChannelName(ServerHandler.DungeonHallName)]
public class ShowAllDungeons : DialogCommandBase<ShowAllDungeonsDialog>
{
    public ShowAllDungeons(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "dungeonlist";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("List all currently known dungeons")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        ShowAllDungeonsDialog dialog)
    {
        dialog.GuildId = new GuildId(context.Guild.Id);
        var allDungeonsResult = await dungeonService.GetAllDungeonsAsync(dialog.GuildId, dialog.Context);
        if (!allDungeonsResult.WasSuccessful)
        {
            await command.RespondAsync(ephemeral: true, embed: new EmbedBuilder().WithTitle("Something went wrong!")
                .WithDescription(allDungeonsResult.ErrorMessage).WithColor(Color.Red).Build());
            return;
        }

        var sb = new StringBuilder();
        foreach (var dungeon in allDungeonsResult.Value.OrderByDescending(d => d.Level.Value)
                     .ThenByDescending(d => d.Rarity))
        {
            sb.AppendLine(
                $"[{dungeon.Rarity}] Lvl.{dungeon.Level} <#{dungeon.Id}> ({dungeon.Explorations})");
        }

        await command.RespondAsync(sb.ToString(), ephemeral: true);
        EndDialog(dialog.UserId);
    }
}