using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class CreateCharacterCommand : Command<GuildAggregate, GuildId>
{
    public CreateCharacterCommand(GuildId aggregateId, Character character) : base(aggregateId)
    {
        Character = character;
    }

    public Character Character { get; private set; }
}

public class CreateCharacterCommandHandler : CommandHandler<GuildAggregate, GuildId, CreateCharacterCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddCharacter(command.Character);
        return Task.CompletedTask;
    }
}