using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommandHandler : CommandHandler<CreateDungeonCommand>
{
    private readonly IDungeonRepository dungeonRepository;

    public CreateDungeonCommandHandler(IMediator mediator, IDungeonRepository dungeonRepository) : base(mediator)
    {
        this.dungeonRepository = dungeonRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateDungeonCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await dungeonRepository.SaveDungeonAsync(request.Dungeon, cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}