using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Shops;

public class DeleteShopCommandHandler : CommandHandler<DeleteShopCommand>
{
    private readonly IRepository<Shop> repository;

    public DeleteShopCommandHandler(IMediator mediator, ILogger logger, IRepository<Shop> repository) : base(mediator,
        logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteShopCommand request, CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var shop = await repository.GetAsync(request.ShopId, cancellationToken);
            if (shop is null)
                return ExecutionResult.Failure($"No Shop with ID {request.ShopId} found");

            await repository.DeleteAsync(request.ShopId, cancellationToken);

            await PublishAsync(new ShopDeleted(shop), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handing failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}