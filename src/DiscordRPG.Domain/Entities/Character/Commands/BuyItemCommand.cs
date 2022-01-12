using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class BuyItemCommand : Command<GuildAggregate, GuildId>
{
    public BuyItemCommand(GuildId aggregateId, CharacterId characterId, Item item) : base(aggregateId)
    {
        CharacterId = characterId;
        Item = item;
    }

    public CharacterId CharacterId { get; private set; }
    public Item Item { get; private set; }
}

public class BuyItemCommandHandler : CommandHandler<GuildAggregate, GuildId, BuyItemCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, BuyItemCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.BuyItem(command.CharacterId, command.Item);
        return Task.CompletedTask;
    }
}