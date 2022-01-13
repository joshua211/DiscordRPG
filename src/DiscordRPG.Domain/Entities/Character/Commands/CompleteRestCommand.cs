using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class CompleteRestCommand : Command<GuildAggregate, GuildId>
{
    public CompleteRestCommand(GuildId aggregateId, CharacterId characterId, TransactionContext context) :
        base(aggregateId)
    {
        CharacterId = characterId;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CompleteRestCommandHandler : CommandHandler<GuildAggregate, GuildId, CompleteRestCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CompleteRestCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CompleteCharacterRest(command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}