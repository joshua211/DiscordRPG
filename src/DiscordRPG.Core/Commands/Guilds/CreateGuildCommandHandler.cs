using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Guilds;

public class CreateGuildCommandHandler : CommandHandler<CreateGuildCommand>
{
    private readonly IRepository<Guild> repository;

    public CreateGuildCommandHandler(IMediator mediator, IRepository<Guild> repository) : base(mediator)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(CreateGuildCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveAsync(request.Guild, cancellationToken);

            await PublishAsync(new GuildCreated(request.Guild), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}