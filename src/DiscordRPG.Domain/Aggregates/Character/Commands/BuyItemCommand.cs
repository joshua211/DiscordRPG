using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class BuyItemCommand : Command<CharacterAggregate, CharacterId>
{
    public BuyItemCommand(CharacterId aggregateId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class BuyItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, BuyItemCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, BuyItemCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.BuyItem(command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}