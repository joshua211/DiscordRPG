using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Guilds;

public class CreateGuildCommandHandler : CommandHandler<CreateGuildCommand>
{
    private readonly IRepository<Guild> repository;

    public CreateGuildCommandHandler(IMediator mediator, IRepository<Guild> repository, ILogger logger) : base(mediator,
        logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(CreateGuildCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", nameof(request));
            await repository.SaveAsync(request.Guild, cancellationToken);

            await PublishAsync(new GuildCreated(request.Guild), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handing failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}