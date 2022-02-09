using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IShopService
{
    Task<Result<ShopReadModel>> GetGuildShopAsync(GuildId guildId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> CreateGuildShopAsync(GuildId guildId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateShopInventoryAsync(GuildId guildId, CharacterId characterId, ShopId shopId,
        IEnumerable<Item> newInventory, TransactionContext context, CancellationToken cancellationToken = default);
}