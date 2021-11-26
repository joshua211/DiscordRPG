using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using Serilog;

namespace DiscordRPG.Application.Services;

public class ChannelManager : IChannelManager
{
    private readonly DiscordSocketClient client;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public ChannelManager(DiscordSocketClient client, IGuildService guildService, ILogger logger)
    {
        this.client = client;
        this.guildService = guildService;
        this.logger = logger;
    }

    public async Task<ulong> CreateDungeonThreadAsync(DiscordId guildId, string dungeonName)
    {
        logger.Information("Creating dungeon channel {Name} for guild {Id}", dungeonName, guildId);
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            logger.Warning("No guild found");
            return 0;
        }

        var socketGuild = client.GetGuild(guildId);
        var cat = socketGuild.CategoryChannels.FirstOrDefault(c => c.Name == "--RPG--");
        var dungeonChannel = socketGuild.GetTextChannel(result.Value.DungeonHallId);
        var thread = await dungeonChannel.CreateThreadAsync(dungeonName);

        logger.Information("Created dungeon channel {Name}", dungeonName);

        return thread.Id;
    }

    public async Task SendToGuildHallAsync(DiscordId guildId, string text)
    {
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            return;
        }

        var channel = client.GetChannel(result.Value.GuildHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text);
    }

    public async Task SendToDungeonHallAsync(DiscordId guildId, string text)
    {
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            return;
        }

        var channel = client.GetChannel(result.Value.DungeonHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text);
    }

    public async Task DeleteDungeonThreadAsync(DiscordId threadId)
    {
        try
        {
            logger.Verbose("Deleting thread {Id}", threadId);
            var channel = client.GetChannel(threadId) as IGuildChannel;
            await channel.DeleteAsync();
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to delete thread {ThreadId}", threadId);
        }
    }

    public async Task UpdateDungeonThreadNameAsync(DiscordId threadId, string name)
    {
        try
        {
            logger.Verbose("Updating Thread {Id} name to {Name}", threadId, name);
            var channel = client.GetChannel(threadId) as SocketGuildChannel;
            await channel.ModifyAsync(properties => properties.Name = name);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to update name of thread {Id}", threadId);
        }
    }
}