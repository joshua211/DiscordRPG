using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class CreateCharacterCommandHandler : CommandHandler<CreateCharacterCommand>
{
    private readonly IRepository<Character> characterRepository;
    private readonly IRepository<Guild> guildRepository;


    public CreateCharacterCommandHandler(IRepository<Character> characterRepository, IMediator mediator,
        IRepository<Guild> guildRepository, ILogger logger) : base(mediator, logger)
    {
        this.characterRepository = characterRepository;
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", command.GetType().Name);

            await characterRepository.SaveAsync(command.Character, cancellationToken);
            await PublishAsync(new CharacterCreated(command.Character), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}