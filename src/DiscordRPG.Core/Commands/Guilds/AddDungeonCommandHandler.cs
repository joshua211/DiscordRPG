using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands.Guilds;

public class AddDungeonCommandHandler : CommandHandler<AddDungeonCommand>
{
    private readonly IGuildRepository guildRepository;

    public AddDungeonCommandHandler(IMediator mediator, IGuildRepository guildRepository) : base(mediator)
    {
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(AddDungeonCommand request, CancellationToken cancellationToken)
    {
        request.Guild.Dungeons.Add(request.Dungeon);

        await guildRepository.UpdateGuildAsync(request.Guild, cancellationToken);

        return ExecutionResult.Success();
    }
}