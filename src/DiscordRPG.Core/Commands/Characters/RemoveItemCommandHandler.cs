using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class RemoveItemCommandHandler : CommandHandler<RemoveItemCommand>
{
    private readonly IRepository<Character> repository;

    public RemoveItemCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> repository) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var item = request.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == request.ItemCode);
            if (item is null)
                return ExecutionResult.Failure("Character does not have this item");

            if (request.Amount >= item.Amount)
                request.Character.Inventory.Remove(item);
            else
                item.Amount -= request.Amount;

            await repository.UpdateAsync(request.Character, cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}