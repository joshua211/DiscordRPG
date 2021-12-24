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

    public override async Task<ExecutionResult> Handle(UpdateShopCommand command, CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", command.GetType().Name);
        try
        {
            command.Shop.UpdateEquipment(command.Character.ID, command.Equipment);

            await repository.UpdateAsync(command.Shop, cancellationToken);

            await PublishAsync(new ShopUpdated(command.Shop), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handing failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}