using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IShopService
{
    Task<Result<Shop>> GetGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result> CreateGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<(Shop, Character)>> BuyEquipAsync(Shop shop, Character character, Equipment equipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Character>> SellItemAsync(Character character, Item item, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Shop>> UpdateWaresAsync(Shop shop, Character character, List<Equipment> newEquipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);
}