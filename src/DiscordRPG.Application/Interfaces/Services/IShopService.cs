using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IShopService
{
    Task<Result<Shop>> GetGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result> CreateGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Shop>> BuyEquipAsync(Identity shopId, Identity characterId, Equipment equipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);

    Task<Result<Shop>> UpdateWaresAsync(Identity shopId, Identity charId, List<Equipment> newEquipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default);
}