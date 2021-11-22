using DiscordRPG.Core.Events;
using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

public class CreateCharacterHandler : CommandHandler<CreateCharacterCommand>
{
    private readonly ICharacterRepository characterRepository;


    public CreateCharacterHandler(ICharacterRepository characterRepository, IMediator mediator) : base(mediator)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await characterRepository.SaveCharacterAsync(command.Character, cancellationToken);

            await mediator.Publish(new CharacterCreated(command.Character), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}