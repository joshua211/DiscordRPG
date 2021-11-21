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

    public override async Task Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        await characterRepository.SaveCharacterAsync(command.Character, cancellationToken);

        await mediator.Publish(new CharacterCreated(command.Character), cancellationToken);
    }
}