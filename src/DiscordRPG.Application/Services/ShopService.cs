using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow;
using EventFlow.Queries;

namespace DiscordRPG.Application.Services;

public class ShopService : IShopService
{
    private readonly ICommandBus bus;
    private readonly ILogger logger;
    private readonly IQueryProcessor queryProcessor;

    public ShopService(ILogger logger, ICommandBus bus, IQueryProcessor queryProcessor)
    {
        this.logger = logger.WithContext(GetType());
        this.bus = bus;
        this.queryProcessor = queryProcessor;
    }

    public async Task<Result<ShopReadModel>> GetGuildShopAsync(GuildId guildId, TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Querying shop for Guild {GuildId}", guildId.Value);
        var query = new GetGuildShopQuery(guildId);
        var result = await queryProcessor.ProcessAsync(query, cancellationToken);

        return Result<ShopReadModel>.Success(result);
    }

    public async Task<Result> CreateGuildShopAsync(GuildId guildId, TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Creating shop for Guild {GuildId}", guildId.Value);
        var shop = new Shop(ShopId.New, new List<SalesInventory>());
        var cmd = new AddShopCommand(guildId, shop, context);

        var result = await bus.PublishAsync(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to create shop for guild {GuildId}", guildId.Value);
        }

        return Result.Success();
    }

    public async Task<Result> UpdateShopInventoryAsync(GuildId guildId, CharacterId characterId, ShopId shopId,
        IEnumerable<Item> newInventory,
        TransactionContext context, CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Updating inventory  for Shop {ShopId} and Character {CharacterId}", shopId,
            characterId);
        var cmd = new UpdateShopInventoryCommand(guildId, shopId, newInventory.ToList(), characterId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context)
                .Error("Failed to update Shop Inventory for Shop {ShopId} and Character {CharacterId}", shopId,
                    characterId);
        }

        return Result.Success();
    }
}