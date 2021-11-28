namespace DiscordRPG.Application.Interfaces;

public interface IChannelManager
{
    Task<ulong> CreateDungeonThreadAsync(DiscordId serverId, string dungeonName);

    Task SendToGuildHallAsync(DiscordId serverId, string text);

    Task SendToDungeonHallAsync(DiscordId serverId, string text);

    Task SendToChannelAsync(DiscordId channelId, string text);

    Task DeleteDungeonThreadAsync(DiscordId threadId);

    Task UpdateDungeonThreadNameAsync(DiscordId threadId, string name);
}