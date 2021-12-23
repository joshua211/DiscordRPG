using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class SellItemCommandHandler : CommandHandler<SellItemCommand>
{
    private readonly IRepository<Character> repository;

    public SellItemCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> repository) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(SellItemCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", command.GetType().Name);
            command.Character.SellItem(command.Item);

            await repository.UpdateAsync(command.Character, cancellationToken);
            await PublishAsync(new ItemBought(command.Character, command.Item), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}