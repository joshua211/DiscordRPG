namespace DiscordRPG.Application.Interfaces.Services;

public interface IGuildMessenger
{
    Task SendToGuildHallAsync(ulong guildId, string text);

    Task SendToDungeonHallAsync(ulong guildId, string text);
}