using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class KillCharacterCommand : Command<GuildAggregate, GuildId>
{
    public KillCharacterCommand(GuildId aggregateId, CharacterId characterId, TransactionContext context) :
        base(aggregateId)
    {
        CharacterId = characterId;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class KillCharacterCommandHandler : CommandHandler<GuildAggregate, GuildId, KillCharacterCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, KillCharacterCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.KillCharacter(command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}