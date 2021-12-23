using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Shops;

public class UpdateShopCommandHandler : CommandHandler<UpdateShopCommand>
{
    private readonly IRepository<Shop> repository;

    public UpdateShopCommandHandler(IMediator mediator, ILogger logger, IRepository<Shop> repository) : base(mediator,
        logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var shop = await repository.GetAsync(request.ShopId, cancellationToken);
            if (shop is null)
                return ExecutionResult.Failure($"No Shop with ID {request.ShopId} found");

            shop.UpdateEquipment(request.CharId, request.Equipment);

            await repository.UpdateAsync(shop, cancellationToken);

            await PublishAsync(new ShopUpdated(shop), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handing failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}