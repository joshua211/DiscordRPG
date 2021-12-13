using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;

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
        this.logger = logger.WithContext(GetType());
    }

    public async Task<ulong> CreateDungeonThreadAsync(DiscordId guildId, string dungeonName)
    {
        logger.Here().Debug("Creating dungeon channel {Name} for guild {Id}", dungeonName, guildId);
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            logger.Here().Warning("No guild found");
            return 0;
        }

        var socketGuild = client.GetGuild(guildId);
        var dungeonChannel = socketGuild.GetTextChannel(result.Value.DungeonHallId);
        var thread = await dungeonChannel.CreateThreadAsync(dungeonName);

        logger.Here().Debug("Created dungeon channel {Name}", dungeonName);

        return thread.Id;
    }

    public async Task SendToGuildHallAsync(DiscordId guildId, string text, Embed embed = null)
    {
        logger.Here().Verbose("Sending Message to GuildHall: {Msg}", text);
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            logger.Here().Warning("No guild found, cant send message");
            return;
        }

        var channel = client.GetChannel(result.Value.GuildHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text, embed: embed);
    }

    public async Task SendToDungeonHallAsync(DiscordId guildId, string text)
    {
        logger.Here().Verbose("Sending Message to GuildHall: {Msg}", text);
        var result = await guildService.GetGuildWithDiscordIdAsync(guildId);
        if (!result.WasSuccessful)
        {
            logger.Here().Warning("No guild found, cant send message");
            return;
        }

        var channel = client.GetChannel(result.Value.DungeonHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text);
    }

    public async Task SendToChannelAsync(DiscordId channelId, string text, Embed embed = null)
    {
        logger.Here().Verbose("Sending Message to Channel: {Channel}", channelId.Value);
        var channel = client.GetChannel(channelId) as SocketTextChannel;
        if (channel is null)
        {
            logger.Here().Warning("No channel found with id {Id}, cant send message", channelId.Value);
            return;
        }

        await channel.SendMessageAsync(text, embed: embed);
    }

    public async Task DeleteDungeonThreadAsync(DiscordId threadId)
    {
        try
        {
            logger.Here().Debug("Deleting thread {Id}", threadId);

            var channel = client.GetChannel(threadId) as IGuildChannel;
            await channel.DeleteAsync();
        }
        catch (Exception)
        {
            logger.Here().Warning("Failed to delete thread {ThreadId}", threadId);
        }
    }

    public async Task UpdateDungeonThreadNameAsync(DiscordId threadId, string name)
    {
        try
        {
            logger.Here().Debug("Updating Thread {Id} name to {Name}", threadId, name);
            var channel = client.GetChannel(threadId) as SocketGuildChannel;
            await channel.ModifyAsync(properties => properties.Name = name);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to update name of thread {Id}", threadId);
        }
    }

    public async Task AddUserToThread(DiscordId threadId, DiscordId userId)
    {
        try
        {
            logger.Here().Debug("Adding user {UserId} to thread {ThreadId}", userId, threadId);
            var thread = client.GetChannel(threadId) as SocketThreadChannel;
            var user = thread.Guild.GetUser(userId);
            await thread.AddUserAsync(user);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to add user to thread");
        }
    }
}