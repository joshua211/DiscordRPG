using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

public class DeleteGuildCommandHandler : CommandHandler<DeleteGuildCommand>
{
    private readonly IGuildRepository guildRepository;

    public DeleteGuildCommandHandler(IMediator mediator, IGuildRepository guildRepository) : base(mediator)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
    {
        await guildRepository.DeleteGuildAsync(request.Id, cancellationToken);

        return ExecutionResult.Success();
    }
}