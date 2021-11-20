using DiscordRPG.Core.Repositories;

namespace DiscordRPG.Core.Commands;

public class CreateCharacterHandler : CommandHandler<CreateCharacterCommand>
{
    private readonly ICharacterRepository characterRepository;

    public CreateCharacterHandler(ICharacterRepository characterRepository)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        await characterRepository.SaveCharacterAsync(command.Character, cancellationToken);
    }
}