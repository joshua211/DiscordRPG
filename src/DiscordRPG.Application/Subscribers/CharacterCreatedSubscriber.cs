using Discord;
using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Character.Events;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Enums;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, CharacterCreated>
{
    private readonly IChannelManager channelManager;
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly IShopService shopService;

    public CharacterCreatedSubscriber(IChannelManager channelManager, IShopService shopService, ILogger logger,
        IItemGenerator itemGenerator)
    {
        this.channelManager = channelManager;
        this.shopService = shopService;
        this.itemGenerator = itemGenerator;
        this.logger = logger.WithContext(GetType());
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, CharacterCreated> domainEvent,
        CancellationToken cancellationToken)
    {
        var context = TransactionContext.With(domainEvent.Metadata["transaction-id"], "CharacterCreatedSubscriber");
        var character = domainEvent.AggregateEvent.Character;
        var embed = new EmbedBuilder().WithTitle("New Member!")
            .WithDescription($"The {character.Race.Name} {character.Class.Name} {character.Name} has joined the guild!")
            .WithColor(Color.Gold).Build();

        await channelManager.SendToGuildHallAsync(domainEvent.AggregateIdentity, String.Empty, context, embed);

        var shop = await shopService.GetGuildShopAsync(domainEvent.AggregateIdentity, context, cancellationToken);
        if (!shop.WasSuccessful)
        {
            logger.Context(context).Error("Failed to get guild shop, cant create sales inventory");
            return;
        }

        var startingInventory = GetStartingShopInventory();
        await shopService.UpdateShopInventoryAsync(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.EntityId,
            new ShopId(shop.Value.Id), startingInventory, context, cancellationToken);
    }

    private IEnumerable<Item> GetStartingShopInventory()
    {
        var aspect = Aspects.OrdinaryAspect;
        for (int i = 0; i < 6; i++)
        {
            yield return itemGenerator.GenerateRandomEquipment(Rarity.Common, 2, aspect);
        }

        for (int i = 0; i < 3; i++)
        {
            yield return itemGenerator.GenerateRandomWeapon(Rarity.Common, 2, aspect);
        }
    }
}