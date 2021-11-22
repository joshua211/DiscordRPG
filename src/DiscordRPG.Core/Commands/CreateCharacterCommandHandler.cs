using DiscordRPG.Core.Events;
using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

public class CreateCharacterCommandHandler : CommandHandler<CreateCharacterCommand>
{
    private readonly ICharacterRepository characterRepository;
    private readonly IGuildRepository guildRepository;


    public CreateCharacterCommandHandler(ICharacterRepository characterRepository, IMediator mediator,
        IGuildRepository guildRepository) : base(mediator)
    {
        this.characterRepository = characterRepository;
        this.guildRepository = guildRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await characterRepository.SaveCharacterAsync(command.Character, cancellationToken);

            var guild = await guildRepository.GetGuildAsync(command.Character.GuildId, cancellationToken);
            guild.Characters.Add(command.Character.UserId);
            await guildRepository.UpdateGuildAsync(guild, cancellationToken);

            await mediator.Publish(new CharacterCreated(command.Character), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}