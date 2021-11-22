using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using Serilog;

namespace DiscordRPG.Client.Handlers;

public class ServerHandler : IHandler
{
    private readonly ApplicationCommandHandler applicationCommandHandler;
    private readonly DiscordSocketClient client;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public ServerHandler(DiscordSocketClient client, IGuildService guildService, ILogger logger,
        ApplicationCommandHandler applicationCommandHandler)
    {
        this.client = client;
        this.guildService = guildService;
        this.logger = logger;
        this.applicationCommandHandler = applicationCommandHandler;
    }

    public Task InstallAsync()
    {
        logger.Information("Installing ServerHandler");
        client.JoinedGuild += SetupServer;
        client.LeftGuild += CleanServer;

        return Task.CompletedTask;
    }

    private async Task CleanServer(SocketGuild socketGuild)
    {
        try
        {
            await guildService.DeleteGuildAsync(socketGuild.Id);

            //TODO delete characters
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to clean up server");
        }
    }

    private async Task SetupServer(SocketGuild socketGuild)
    {
        try
        {
            logger.Information("Installing guild commands");
            await InstallGuildCommands(socketGuild);

            logger.Information("Setting up channels for Guild {Id}", socketGuild.Id);
            await guildService.DeleteGuildAsync(socketGuild.Id);

            var category = await socketGuild.CreateCategoryChannelAsync("--RPG--");

            var guildHall =
                await socketGuild.CreateTextChannelAsync("Guild Hall",
                    properties => properties.CategoryId = category.Id);
            var dungeonHall =
                await socketGuild.CreateTextChannelAsync("Dungeon Hall",
                    properties => properties.CategoryId = category.Id);

            var result =
                await guildService.CreateGuildAsync(socketGuild.Id, socketGuild.Name, guildHall.Id, dungeonHall.Id);

            if (result.WasSuccessful)
            {
                logger.Information("Created Guild {Id}", socketGuild.Id);
                return;
            }

            logger.Warning("Failed to create Guild. {Msg}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to create Guild");
        }
    }

    public async Task InstallGuildCommands(SocketGuild guild)
    {
        foreach (var cmd in applicationCommandHandler.LoadedCommands)
        {
            await cmd.InstallAsync(guild);
        }
    }
}