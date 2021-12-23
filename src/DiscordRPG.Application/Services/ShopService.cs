using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Shops;
using MediatR;

namespace DiscordRPG.Application.Services;

public class ShopService : ApplicationService, IShopService
{
    public ShopService(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Result<Shop>> GetGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetShopByGuildIdQuery(guildId);
            var result = await ProcessAsync(ctx, query, cancellationToken);

            return Result<Shop>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Shop>.Failure(e.Message);
        }
    }

    public async Task<Result> CreateGuildShopAsync(Identity guildId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var cmd = new CreateShopCommand(guildId);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result<Shop>.Failure(result.ErrorMessage);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Shop>.Failure(e.Message);
        }
    }

    public async Task<Result<Shop>> BuyEquipAsync(Identity shopId, Identity characterId, Equipment equipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetShopQuery(shopId);
            var shop = await ProcessAsync(ctx, query, cancellationToken);
            if (shop is null)
            {
                TransactionError(ctx, "No Shop found with id {Id}, cant buy equipment", shopId);
                return Result<Shop>.Failure("No shop found");
            }

            var itemsForSale = shop[characterId];
            if (itemsForSale is null)
            {
                TransactionError(ctx, "No shop for user with Id {Id}", characterId);
                return Result<Shop>.Failure("No shop found");
            }

            if (!itemsForSale.Contains(equipment))
            {
                TransactionWarning(ctx, "Shop with Id {Id} does not have the required equipment");

                return Result<Shop>.Failure("The selected item is not available");
            }


            itemsForSale.Remove(equipment);
            var cmd = new UpdateShopCommand(shop.ID, characterId, itemsForSale);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result<Shop>.Failure("Something went wrong while trying to buy the item, please try again");
            }

            return Result<Shop>.Success(shop);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Shop>.Failure(e.Message);
        }
    }

    public async Task<Result<Shop>> UpdateWaresAsync(Identity shopId, Identity charId, List<Equipment> newEquipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetShopQuery(shopId);
            var shop = await ProcessAsync(ctx, query, cancellationToken);
            if (shop is null)
            {
                TransactionError(ctx, "No Shop found with id {Id}, cant buy equipment", shopId);
                return Result<Shop>.Failure("No shop found");
            }

            var cmd = new UpdateShopCommand(shop.ID, charId, newEquipment);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result<Shop>.Failure("Something went wrong while trying to update the shop inventory");
            }

            shop.UpdateEquipment(charId, newEquipment);

            return Result<Shop>.Success(shop);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Shop>.Failure(e.Message);
        }
    }
}