using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using Polly;
using Polly.Retry;

namespace DiscordRPG.Application.Services;

public class ChannelManager : IChannelManager
{
    private readonly DiscordSocketClient client;
    private readonly IGuildService guildService;
    private readonly ILogger logger;
    private readonly AsyncRetryPolicy policy;

    public ChannelManager(DiscordSocketClient client, IGuildService guildService, ILogger logger)
    {
        this.client = client;
        this.guildService = guildService;
        this.logger = logger.WithContext(GetType());

        policy = Policy.Handle<Exception>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
            },
            (exception, timeSpan, retryCount, context) =>
            {
                logger.Warning("Discord Interaction failed {Count} times: {Message}", retryCount,
                    exception.Message);
            });
    }

    public async Task<ChannelId> CreateDungeonThreadAsync(GuildId guildId, string dungeonName,
        TransactionContext context)
    {
        logger.Context(context).Verbose("Creating dungeon channel {Name} for guild {Id}",
            new object[] {dungeonName, guildId});
        var result = await guildService.GetGuildAsync(guildId, context);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Verbose("No guild found");
            return null;
        }

        return await policy.ExecuteAsync(async () =>
        {
            var socketGuild = client.GetGuild(ulong.Parse(guildId.Value));
            var dungeonChannel = socketGuild.GetTextChannel(result.Value.DungeonHallId);
            var thread = await dungeonChannel.CreateThreadAsync(dungeonName);

            logger.Context(context).Debug("Created dungeon channel {Name}", dungeonName);

            return new ChannelId(thread.Id.ToString());
        });
    }

    public async Task SendToGuildHallAsync(GuildId guildId, string text, TransactionContext context, Embed embed = null)
    {
        logger.Context(context).Verbose("Sending Message to GuildHall");
        var result = await guildService.GetGuildAsync(guildId, context);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Warning("No guild found, cant send message");
            return;
        }

        await policy.ExecuteAsync(async () =>
        {
            var channel = client.GetChannel(result.Value.GuildHallId) as SocketTextChannel;
            if (channel is not null)
                await channel.SendMessageAsync(text, embed: embed);
        });
    }

    public async Task SendToInn(GuildId guildId, string text, TransactionContext context, Embed embed = null)
    {
        logger.Context(context).Verbose("Sending Message to Inn");
        var result = await guildService.GetGuildAsync(guildId, context);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Warning("No guild found, cant send message");
            return;
        }

        await policy.ExecuteAsync(async () =>
        {
            var channel = client.GetChannel(result.Value.InnId) as SocketTextChannel;
            if (channel is not null)
                await channel.SendMessageAsync(text, embed: embed);
        });
    }

    public async Task SendToDungeonHallAsync(GuildId guildId, string text, TransactionContext context,
        Embed embed = null)
    {
        logger.Context(context).Verbose("Sending Message to GuildHall");
        var result = await guildService.GetGuildAsync(guildId, context);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Warning("No guild found, cant send message");
            return;
        }

        await policy.ExecuteAsync(async () =>
        {
            var channel = client.GetChannel(result.Value.DungeonHallId) as SocketTextChannel;
            if (channel is not null)
                await channel.SendMessageAsync(text, embed: embed);
        });
    }

    public async Task SendToChannelAsync(GuildId guildId, ChannelId channelId, string text, TransactionContext context,
        Embed embed = null)
    {
        logger.Context(context).Verbose("Sending Message to Channel: {Channel}", channelId.Value);
        var channel = client.GetChannel(channelId) as SocketTextChannel;
        if (channel is null)
        {
            logger.Context(context).Warning("No channel found with id {Id}, cant send message", channelId.Value);
            return;
        }

        await policy.ExecuteAsync(async () => { await channel.SendMessageAsync(text, embed: embed); });
    }

    public async Task DeleteDungeonThreadAsync(ChannelId channelId, TransactionContext context)
    {
        try
        {
            logger.Context(context).Debug("Deleting thread {Id}", channelId.Value);

            await policy.ExecuteAsync(async () =>
            {
                var channel = client.GetChannel(channelId) as IGuildChannel;
                await channel.DeleteAsync();
            });
        }
        catch (Exception)
        {
            logger.Context(context).Warning("Failed to delete thread {ThreadId}", channelId.Value);
        }
    }

    public async Task UpdateDungeonThreadNameAsync(ChannelId channelId, string name, TransactionContext context)
    {
        try
        {
            logger.Context(context).Debug("Updating Thread {Id} name to {Name}", channelId, name);
            await policy.ExecuteAsync(async () =>
            {
                var channel = client.GetChannel(channelId) as SocketGuildChannel;
                await channel.ModifyAsync(properties => properties.Name = name);
            });
        }
        catch (Exception e)
        {
            logger.Context(context).Error(e, "Failed to update name of thread {Id}", channelId);
        }
    }

    public async Task AddUserToThread(ChannelId channelId, CharacterId characterId, TransactionContext context)
    {
        try
        {
            logger.Context(context).Debug("Adding user {UserId} to thread {ThreadId}", characterId.Value, channelId);
            await policy.ExecuteAsync(async () =>
            {
                var thread = client.GetChannel(channelId) as SocketThreadChannel;
                var user = thread.Guild.GetUser(ulong.Parse(characterId.Value));
                await thread.AddUserAsync(user);
            });
        }
        catch (Exception e)
        {
            logger.Context(context).Error(e, "Failed to add user to thread");
        }
    }
}