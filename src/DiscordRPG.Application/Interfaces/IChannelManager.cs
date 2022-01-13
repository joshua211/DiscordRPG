using Discord;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character;

namespace DiscordRPG.Application.Interfaces;

public interface IChannelManager
{
    Task<ChannelId> CreateDungeonThreadAsync(GuildId guildId, string dungeonName, TransactionContext context);

    Task SendToGuildHallAsync(GuildId guildId, string text, TransactionContext context, Embed embed = null);
    Task SendToInn(GuildId guildId, string text, TransactionContext context, Embed embed = null);

    Task SendToDungeonHallAsync(GuildId guildId, string text, TransactionContext context, Embed embed = null);

    Task SendToChannelAsync(GuildId guildId, ChannelId channelId, string text, TransactionContext context,
        Embed embed = null);

    Task DeleteDungeonThreadAsync(ChannelId channelId, TransactionContext context);

    Task UpdateDungeonThreadNameAsync(ChannelId channelId, string name, TransactionContext context);
    Task AddUserToThread(ChannelId channelId, CharacterId characterId, TransactionContext context);
}