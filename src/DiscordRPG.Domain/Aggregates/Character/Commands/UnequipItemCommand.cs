using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class UnequipItemCommand : Command<CharacterAggregate, CharacterId>
{
    public UnequipItemCommand(CharacterId aggregateId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class UnequipItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, UnequipItemCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, UnequipItemCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.UnequipItem(command.CharacterId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}