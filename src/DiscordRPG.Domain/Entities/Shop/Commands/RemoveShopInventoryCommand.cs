using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Shop.Commands;

public class RemoveShopInventoryCommand : Command<GuildAggregate, GuildId>
{
    public RemoveShopInventoryCommand(GuildId aggregateId, ShopId shopId, CharacterId characterId,
        TransactionContext context) : base(aggregateId)
    {
        ShopId = shopId;
        CharacterId = characterId;
        Context = context;
    }

    public ShopId ShopId { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class RemoveShopInventoryCommandHandler : CommandHandler<GuildAggregate, GuildId, RemoveShopInventoryCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, RemoveShopInventoryCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RemoveShopInventory(command.ShopId, command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}