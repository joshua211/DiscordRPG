using Discord;
using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : EventSubscriber<CharacterCreated>
{
    private readonly IChannelManager channelManager;
    private readonly IGuildService guildService;
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly IShopService shopService;

    public CharacterCreatedSubscriber(ILogger logger, IChannelManager channelManager, IGuildService guildService,
        IShopService shopService, IItemGenerator itemGenerator)
    {
        this.logger = logger.WithContext(GetType());
        this.channelManager = channelManager;
        this.guildService = guildService;
        this.shopService = shopService;
        this.itemGenerator = itemGenerator;
    }

    public override async Task Handle(CharacterCreated domainEvent, CancellationToken cancellationToken)
    {
        var guildResult =
            await guildService.GetGuildAsync(domainEvent.Character.GuildId, cancellationToken: cancellationToken);
        if (!guildResult.WasSuccessful)
        {
            logger.Here().Warning("No guild found to complete the event {Name}", domainEvent.GetType().Name);
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("New Member!")
            .WithColor(Color.Gold)
            .WithDescription(
                $"The {domainEvent.Character.CharacterRace.RaceName} {domainEvent.Character.CharacterClass.ClassName} {domainEvent.Character.CharacterName} has joined the guild!")
            .Build();

        await channelManager.SendToGuildHallAsync(guildResult.Value.ServerId, "", embed);

        var shopResult =
            await shopService.GetGuildShopAsync(guildResult.Value.ID, cancellationToken: cancellationToken);
        if (!shopResult.WasSuccessful)
        {
            logger.Here().Warning("No shop found for guild {Id}, cant create shop inventory for player");
            return;
        }

        var equip = GetStartingShopInventory();
        await shopService.UpdateWaresAsync(shopResult.Value, domainEvent.Character, equip.ToList(),
            cancellationToken: cancellationToken);
    }

    private IEnumerable<Equipment> GetStartingShopInventory()
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