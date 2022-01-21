using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Shop;

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