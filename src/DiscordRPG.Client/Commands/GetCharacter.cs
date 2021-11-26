using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Handlers;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireChannelName(ServerHandler.GuildHallName)]
[RequireGuild]
[RequireCharacter]
public class GetCharacter : CommandBase
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
            logger.Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleAsync(SocketSlashCommand command, GuildCommandContext context)
    {
        await command.RespondAsync(context.Character!.CharacterName);
    }
}