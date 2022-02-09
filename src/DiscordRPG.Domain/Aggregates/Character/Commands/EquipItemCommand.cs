using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class EquipItemCommand : Command<CharacterAggregate, CharacterId>
{
    public EquipItemCommand(CharacterId aggregateId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class EquipItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, EquipItemCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, EquipItemCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.EquipItem(command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}