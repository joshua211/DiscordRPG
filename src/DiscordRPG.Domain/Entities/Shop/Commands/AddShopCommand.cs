using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Shop.Commands;

public class AddShopCommand : Command<GuildAggregate, GuildId>
{
    public AddShopCommand(GuildId aggregateId, Shop shop) : base(aggregateId)
    {
        Shop = shop;
    }

    public Shop Shop { get; private set; }
}

public class AddShopCommandHandler : CommandHandler<GuildAggregate, GuildId, AddShopCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddShopCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddShop(command.Shop);
        return Task.CompletedTask;
    }
}