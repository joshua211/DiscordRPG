using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Shop.Commands;

public class UpdateShopInventoryCommand : Command<GuildAggregate, GuildId>
{
    public UpdateShopInventoryCommand(GuildId aggregateId, ShopId shopId, List<Item> newInventory,
        CharacterId characterId, TransactionContext context) : base(aggregateId)
    {
        ShopId = shopId;
        NewInventory = newInventory;
        CharacterId = characterId;
        Context = context;
    }

    public ShopId ShopId { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public List<Item> NewInventory { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class UpdateShopInventoryCommandHandler : CommandHandler<GuildAggregate, GuildId, UpdateShopInventoryCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, UpdateShopInventoryCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateShopInventory(command.ShopId, command.NewInventory, command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}