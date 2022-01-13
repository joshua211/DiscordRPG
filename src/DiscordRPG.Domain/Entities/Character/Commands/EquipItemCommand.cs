using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class EquipItemCommand : Command<GuildAggregate, GuildId>
{
    public EquipItemCommand(GuildId aggregateId, CharacterId characterId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        CharacterId = characterId;
        ItemId = itemId;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class EquipItemCommandHandler : CommandHandler<GuildAggregate, GuildId, EquipItemCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, EquipItemCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.EquipItem(command.CharacterId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}