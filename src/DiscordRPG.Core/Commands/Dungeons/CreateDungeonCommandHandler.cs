using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommandHandler : CommandHandler<CreateDungeonCommand>
{
    private readonly IRepository<Dungeon> dungeonRepository;

    public CreateDungeonCommandHandler(IMediator mediator, IRepository<Dungeon> dungeonRepository) : base(mediator)
    {
        this.dungeonRepository = dungeonRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateDungeonCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await dungeonRepository.SaveAsync(request.Dungeon, cancellationToken);

            await PublishAsync(new DungeonCreated(request.Dungeon), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}