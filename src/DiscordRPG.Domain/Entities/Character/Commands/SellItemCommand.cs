using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class SellItemCommand : Command<GuildAggregate, GuildId>
{
    public SellItemCommand(GuildId aggregateId, CharacterId entityId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        EntityId = entityId;
        ItemId = itemId;
        Context = context;
    }

    public CharacterId EntityId { get; private set; }
    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class SellItemCommandHandler : CommandHandler<GuildAggregate, GuildId, SellItemCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, SellItemCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SellItem(command.EntityId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}