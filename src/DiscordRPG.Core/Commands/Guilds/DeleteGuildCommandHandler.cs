using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Guilds;

public class DeleteGuildCommandHandler : CommandHandler<DeleteGuildCommand>
{
    private readonly IRepository<Character> characterRepository;
    private readonly IRepository<Guild> guildRepository;

    public DeleteGuildCommandHandler(IMediator mediator, IRepository<Guild> guildRepository,
        IRepository<Character> characterRepository, ILogger logger) : base(mediator, logger)
    {
        this.guildRepository = guildRepository;
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", request.GetType().Name);

            var guild = (await guildRepository.FindAsync(g => g.ServerId.Value == request.Id.Value, cancellationToken))
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
            logger.Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}