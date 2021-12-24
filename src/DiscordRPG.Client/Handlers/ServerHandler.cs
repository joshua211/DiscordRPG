﻿using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common.Extensions;
using Serilog;

namespace DiscordRPG.Client.Handlers;

public class ServerHandler : IHandler
{
    public const string GuildHallName = "guild-hall";
    public const string DungeonHallName = "dungeon-hall";
    public const string CategoryName = "--RPG--";

    private readonly ApplicationCommandHandler applicationCommandHandler;
    private readonly ICharacterService characterService;
    private readonly DiscordSocketClient client;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public ServerHandler(DiscordSocketClient client, IGuildService guildService, ILogger logger,
        ApplicationCommandHandler applicationCommandHandler, ICharacterService characterService)
    {
        this.client = client;
        this.guildService = guildService;
        this.logger = logger.WithContext(GetType());
        this.applicationCommandHandler = applicationCommandHandler;
        this.characterService = characterService;
    }

    public Task InstallAsync()
    {
        logger.Here().Information("Installing ServerHandler");
        client.JoinedGuild += SetupServer;
        client.LeftGuild += CleanServer;

        return Task.CompletedTask;
    }

    public async Task CleanServer(SocketGuild socketGuild)
    {
        try
        {
            logger.Here().Debug("Cleaning server {Id}", socketGuild.Id);
            var result = await guildService.GetGuildWithDiscordIdAsync(socketGuild.Id.ToString());
            if (!result.WasSuccessful)
            {
                return;
            }

            await socketGuild.GetChannel(result.Value.GuildHallId).DeleteAsync();
            await socketGuild.GetChannel(result.Value.DungeonHallId).DeleteAsync();
            var cat = socketGuild.CategoryChannels.FirstOrDefault(c => c.Name == CategoryName);
            await cat?.DeleteAsync();

            await guildService.DeleteGuildAsync(socketGuild.Id.ToString());
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to clean up server");
        }
    }

    public async Task SetupServer(SocketGuild socketGuild)
    {
        try
        {
            logger.Here().Debug("Installing guild commands");
            await InstallGuildCommands(socketGuild);

            logger.Here().Debug("Setting up channels for Guild {Id}", socketGuild.Id);
            await guildService.DeleteGuildAsync(socketGuild.Id.ToString());

            var category = await socketGuild.CreateCategoryChannelAsync(CategoryName);

            var guildHall =
                await socketGuild.CreateTextChannelAsync(GuildHallName,
                    properties => properties.CategoryId = category.Id);
            var dungeonHall =
                await socketGuild.CreateTextChannelAsync(DungeonHallName,
                    properties => properties.CategoryId = category.Id);

            var result =
                await guildService.CreateGuildAsync(socketGuild.Id.ToString(), socketGuild.Name,
                    guildHall.Id.ToString(), dungeonHall.Id.ToString());

            if (result.WasSuccessful)
            {
                logger.Here().Debug("Created Guild {Id}", socketGuild.Id);
                return;
            }

            logger.Here().Warning("Failed to create Guild. {Msg}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to create Guild");
        }
    }

    public async Task InstallGuildCommands(SocketGuild guild)
    {
        foreach (var cmd in applicationCommandHandler.LoadedCommands)
        {
            logger.Here().Debug("Installing command {Name}", cmd.CommandName);
            await cmd.InstallAsync(guild);
        }

        logger.Here().Debug("Successfully installed all commands");
    }
}