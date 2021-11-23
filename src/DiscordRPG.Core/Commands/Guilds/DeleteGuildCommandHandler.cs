using DiscordRPG.Core.Events;
using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands.Guilds;

public class DeleteGuildCommandHandler : CommandHandler<DeleteGuildCommand>
{
    private readonly ICharacterRepository characterRepository;
    private readonly IGuildRepository guildRepository;

    public DeleteGuildCommandHandler(IMediator mediator, IGuildRepository guildRepository,
        ICharacterRepository characterRepository) : base(mediator)
    {
        this.guildRepository = guildRepository;
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
    {
        var guild = await guildRepository.GetGuildAsync(request.Id, cancellationToken);
        foreach (var charId in guild.Characters)
        {
            await characterRepository.DeleteCharacterAsync(charId, cancellationToken);
        }

        await guildRepository.DeleteGuildAsync(request.Id, cancellationToken);
        await mediator.Publish(new GuildDeleted(guild), cancellationToken: cancellationToken);

        return ExecutionResult.Success();
    }
}