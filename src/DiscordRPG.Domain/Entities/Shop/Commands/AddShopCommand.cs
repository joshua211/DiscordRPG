using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Shop.Commands;

public class AddShopCommand : Command<GuildAggregate, GuildId>
{
    public AddShopCommand(GuildId aggregateId, Shop shop, TransactionContext context) : base(aggregateId)
    {
        Shop = shop;
        Context = context;
    }

    public Shop Shop { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AddShopCommandHandler : CommandHandler<GuildAggregate, GuildId, AddShopCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddShopCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddShop(command.Shop, command.Context);
        return Task.CompletedTask;
    }
}