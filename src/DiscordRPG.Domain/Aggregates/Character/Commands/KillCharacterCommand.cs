using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class KillCharacterCommand : Command<CharacterAggregate, CharacterId>
{
    public KillCharacterCommand(CharacterId aggregateId, TransactionContext context) :
        base(aggregateId)
    {
        Context = context;
    }

    public TransactionContext Context { get; private set; }
}

public class KillCharacterCommandHandler : CommandHandler<CharacterAggregate, CharacterId, KillCharacterCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, KillCharacterCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.KillCharacter(command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}