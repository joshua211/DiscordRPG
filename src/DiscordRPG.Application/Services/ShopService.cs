using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Characters;
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

    public async Task<Result<(Shop, Character)>> BuyEquipAsync(Shop shop, Character character, Equipment equipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var characterId = character.ID;
            var itemsForSale = shop[characterId];
            if (itemsForSale is null)
            {
                TransactionError(ctx, "No shop for user with Id {Id}", characterId);
                return Result<(Shop, Character)>.Failure("No shop found");
            }

            if (!itemsForSale.Contains(equipment))
            {
                TransactionWarning(ctx, "Shop with Id {Id} does not have the required equipment");

                return Result<(Shop, Character)>.Failure("The selected item is not available");
            }

            if (character.Money < equipment.Worth)
            {
                TransactionDebug(ctx, "Character with ID {ID} not enough money to buy equipment ({Has}/{Needs})",
                    character.ID, character.Money, equipment.Worth);

                return Result<(Shop, Character)>.Failure("You dont have enough money to buy this item!");
            }

            itemsForSale.Remove(equipment);
            var cmd = new UpdateShopCommand(itemsForSale, shop, character);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result<(Shop, Character)>.Failure(
                    "Something went wrong while trying to buy the item, please try again");
            }

            var buyItemCommand = new BuyItemCommand(equipment, character);
            var buyItemResult = await PublishAsync(ctx, buyItemCommand, cancellationToken);
            if (!buyItemResult.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);
                return Result<(Shop, Character)>.Failure(
                    "Something went wrong while trying to buy the item, please try again");
            }

            return Result<(Shop, Character)>.Success(new(shop, character));
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<(Shop, Character)>.Failure(e.Message);
        }
    }

    public async Task<Result<Shop>> UpdateWaresAsync(Shop shop, Character character, List<Equipment> newEquipment,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var cmd = new UpdateShopCommand(newEquipment, shop, character);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result<Shop>.Failure("Something went wrong while trying to update the shop inventory");
            }

            shop.UpdateEquipment(character.ID, newEquipment);

            return Result<Shop>.Success(shop);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Shop>.Failure(e.Message);
        }
    }
}