using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class SellItemCommand : Command<CharacterAggregate, CharacterId>
{
    public SellItemCommand(CharacterId aggregateId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class SellItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, SellItemCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, SellItemCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.SellItem(command.EntityId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}