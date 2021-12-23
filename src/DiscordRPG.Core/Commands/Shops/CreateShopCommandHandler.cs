using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Shops;

public class CreateShopCommandHandler : CommandHandler<CreateShopCommand>
{
    private readonly IRepository<Shop> repository;

    public CreateShopCommandHandler(IMediator mediator, ILogger logger, IRepository<Shop> repository) : base(mediator,
        logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var shop = new Shop(request.GuildId, new List<ShopInventory>());
            await repository.SaveAsync(shop, cancellationToken);

            await PublishAsync(new ShopCreated(shop), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handing failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}