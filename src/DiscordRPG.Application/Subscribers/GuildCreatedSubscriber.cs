using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.Events;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DiscordRPG.Application.Subscribers;

public class GuildCreatedSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, GuildCreated>
{
    private readonly ILogger logger;
    private readonly IShopService shopService;

    public GuildCreatedSubscriber(IShopService shopService, ILogger logger)
    {
        this.shopService = shopService;
        this.logger = logger;
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, GuildCreated> domainEvent,
        CancellationToken cancellationToken)
    {
        var context = TransactionContext.With(domainEvent.Metadata["transaction-id"]);
        var result = await shopService.CreateGuildShopAsync(domainEvent.AggregateIdentity, context, cancellationToken);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Error("Failed to create shop for guild {GuildId}",
                domainEvent.AggregateIdentity.Value);
            return;
        }

        logger.Context(context).Information("Created shop for guild {Id}", domainEvent.AggregateIdentity.Value);
    }
}