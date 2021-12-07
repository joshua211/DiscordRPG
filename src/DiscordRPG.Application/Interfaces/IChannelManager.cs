using Discord;

namespace DiscordRPG.Application.Interfaces;

public interface IChannelManager
{
    Task<ulong> CreateDungeonThreadAsync(DiscordId serverId, string dungeonName);

    Task SendToGuildHallAsync(DiscordId serverId, string text, Embed embed = null);

    Task SendToDungeonHallAsync(DiscordId serverId, string text);

    Task SendToChannelAsync(DiscordId channelId, string text, Embed embed = null);

    Task DeleteDungeonThreadAsync(DiscordId threadId);

    Task UpdateDungeonThreadNameAsync(DiscordId threadId, string name);
    Task AddUserToThread(DiscordId threadId, DiscordId userId);
}