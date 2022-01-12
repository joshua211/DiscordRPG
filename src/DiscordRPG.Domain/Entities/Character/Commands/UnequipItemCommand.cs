using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class UnequipItemCommand : Command<GuildAggregate, GuildId>
{
    public UnequipItemCommand(GuildId aggregateId, CharacterId characterId, ItemId itemId) : base(aggregateId)
    {
        CharacterId = characterId;
        ItemId = itemId;
    }

    public CharacterId CharacterId { get; private set; }
    public ItemId ItemId { get; private set; }
}

public class UnequipItemCommandHandler : CommandHandler<GuildAggregate, GuildId, UnequipItemCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, UnequipItemCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UnequipItem(command.CharacterId, command.ItemId);
        return Task.CompletedTask;
    }
}