using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;

namespace DiscordRPG.Application.Services;

public class GuildMessenger : IGuildMessenger
{
    private readonly DiscordSocketClient client;
    private readonly IGuildService guildService;

    public GuildMessenger(DiscordSocketClient client, IGuildService guildService)
    {
        this.client = client;
        this.guildService = guildService;
    }

    public async Task SendToGuildHallAsync(ulong guildId, string text)
    {
        var result = await guildService.GetGuildAsync(guildId);
        if (!result.WasSuccessful)
        {
            return;
        }

        var channel = client.GetChannel(result.Value.GuildHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text);
    }

    public async Task SendToDungeonHallAsync(ulong guildId, string text)
    {
        var result = await guildService.GetGuildAsync(guildId);
        if (!result.WasSuccessful)
        {
            return;
        }

        var channel = client.GetChannel(result.Value.DungeonHallId) as SocketTextChannel;
        if (channel is not null)
            await channel.SendMessageAsync(text);
    }
}