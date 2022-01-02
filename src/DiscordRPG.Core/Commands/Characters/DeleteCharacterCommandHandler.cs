using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class DeleteCharacterCommandHandler : CommandHandler<DeleteCharacterCommand>
{
    private readonly IRepository<Character> repository;

    public DeleteCharacterCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> repository) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteCharacterCommand command,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", command.GetType().Name);
        try
        {
            await repository.DeleteAsync(command.CharId, cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}