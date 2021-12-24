using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class GuildCreatedSubscriber : EventSubscriber<GuildCreated>
{
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly IShopService shopService;

    public GuildCreatedSubscriber(IShopService shopService, ILogger logger, IItemGenerator itemGenerator)
    {
        this.shopService = shopService;
        this.itemGenerator = itemGenerator;
        this.logger = logger.WithContext(GetType());
    }

    public override async Task Handle(GuildCreated domainEvent, CancellationToken cancellationToken)
    {
        var createShopResult =
            await shopService.CreateGuildShopAsync(domainEvent.Guild.ID, cancellationToken: cancellationToken);

        if (!createShopResult.WasSuccessful)
        {
            logger.Here().Warning("Failed to create shop after Guild creation");
            return;
        }
    }
}