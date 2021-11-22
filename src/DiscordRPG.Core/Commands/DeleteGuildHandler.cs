using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

public class DeleteGuildHandler : CommandHandler<DeleteGuildCommand>
{
    private readonly IGuildRepository guildRepository;

    public DeleteGuildHandler(IMediator mediator, IGuildRepository guildRepository) : base(mediator)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
    {
        await guildRepository.DeleteGuildAsync(request.Id, cancellationToken);

        return ExecutionResult.Success();
    }
}