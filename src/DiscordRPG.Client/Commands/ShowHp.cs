using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Common.Extensions;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class ShowHp : CommandBase
{
    public ShowHp(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client, logger, activityService, characterService, dungeonService,
        guildService, shopService)
    {
    }

    public override string CommandName => "hp";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Show your current hp")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleAsync(SocketSlashCommand command, GuildCommandContext context)
    {
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithTitle($"{context.Character.Name}'s health points");
        embedBuilder.WithDescription($"{context.Character.CurrentHealth}/{context.Character.MaxHealth}");

        await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
}