using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Characters;

public class CreateCharacterCommandHandler : CommandHandler<CreateCharacterCommand>
{
    private readonly IRepository<Character> characterRepository;
    private readonly IRepository<Guild> guildRepository;


    public CreateCharacterCommandHandler(IRepository<Character> characterRepository, IMediator mediator,
        IRepository<Guild> guildRepository) : base(mediator)
    {
        this.characterRepository = characterRepository;
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await characterRepository.SaveAsync(command.Character, cancellationToken);
//TODO do this in an event
            var guild = await guildRepository.GetAsync(command.Character.GuildId, cancellationToken);
            guild.Characters.Add(command.Character.ID);
            await guildRepository.UpdateAsync(guild, cancellationToken);

            await PublishAsync(new CharacterCreated(command.Character), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}