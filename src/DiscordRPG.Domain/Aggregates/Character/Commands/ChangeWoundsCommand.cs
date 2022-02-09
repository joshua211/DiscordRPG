using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class ChangeWoundsCommand : Command<CharacterAggregate, CharacterId>
{
    public ChangeWoundsCommand(CharacterId aggregateId, List<Wound> newWounds,
        TransactionContext context) : base(aggregateId)
    {
        NewWounds = newWounds;
        Context = context;
    }

    public List<Wound> NewWounds { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ChangeWoundsCommandHandler : CommandHandler<CharacterAggregate, CharacterId, ChangeWoundsCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, ChangeWoundsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ChangeWounds(command.NewWounds, command.Context);
        return Task.CompletedTask;
    }
}