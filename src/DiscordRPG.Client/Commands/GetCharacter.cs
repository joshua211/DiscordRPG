using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Handlers;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireChannelName(ServerHandler.GuildHallName)]
public class GetCharacter : CommandBase
{
    private readonly ICharacterService characterService;

    public GetCharacter(DiscordSocketClient client, ILogger logger, ICharacterService characterService) : base(client,
        logger)
    {
        this.characterService = characterService;
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

    public override async Task HandleAsync(SocketSlashCommand command)
    {
        if (!await ShouldExecuteAsync(command))
            return;

        var guildUser = command.User as SocketGuildUser;
        var result = await characterService.GetCharacterAsync(guildUser.Id, guildUser.Guild.Id);

        if (!result.WasSuccessful)
        {
            await command.RespondAsync("You have to first create a character by using /create-character");
            return;
        }

        await command.RespondAsync(result.Value.CharacterName);
    }
}