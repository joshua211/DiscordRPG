using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Guilds;

public class DeleteGuildCommandHandler : CommandHandler<DeleteGuildCommand>
{
    private readonly IRepository<Character> characterRepository;
    private readonly IRepository<Guild> guildRepository;

    public DeleteGuildCommandHandler(IMediator mediator, IRepository<Guild> guildRepository,
        IRepository<Character> characterRepository) : base(mediator)
    {
        this.guildRepository = guildRepository;
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var guild = (await guildRepository.FindAsync(g => g.ServerId == request.Id, cancellationToken))
                .FirstOrDefault();
            if (guild is null)
                return ExecutionResult.Success();

            foreach (var charId in guild.Characters)
            {
                await characterRepository.DeleteAsync(charId, cancellationToken);
            }

            await guildRepository.DeleteAsync(guild.ID, cancellationToken);
            await PublishAsync(new GuildDeleted(guild), cancellationToken: cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}