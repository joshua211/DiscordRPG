namespace DiscordRPG.Application.Interfaces;

public interface IChannelManager
{
    Task<ulong> CreateDungeonThreadAsync(ulong guildId, string dungeonName);

    Task SendToGuildHallAsync(ulong guildId, string text);

    Task SendToDungeonHallAsync(ulong guildId, string text);

    Task DeleteDungeonThreadAsync(ulong threadId);

    Task UpdateDungeonThreadNameAsync(ulong threadId, string name);
}