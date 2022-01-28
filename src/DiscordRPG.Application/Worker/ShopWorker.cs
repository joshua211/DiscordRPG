using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Shop;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Worker;

public class ShopWorker
{
    private readonly AspectGenerator aspectGenerator;
    private readonly ICharacterService characterService;
    private readonly IGuildService guildService;
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly RarityGenerator rarityGenerator;
    private readonly IShopService shopService;

    public ShopWorker(IShopService shopService, IItemGenerator itemGenerator, ICharacterService characterService,
        IGuildService guildService, RarityGenerator rarityGenerator, AspectGenerator aspectGenerator, ILogger logger)
    {
        this.shopService = shopService;
        this.itemGenerator = itemGenerator;
        this.characterService = characterService;
        this.guildService = guildService;
        this.rarityGenerator = rarityGenerator;
        this.aspectGenerator = aspectGenerator;
        this.logger = logger;
    }

    public async Task UpdateShopsAsync()
    {
        var context = TransactionContext.New();
        var guilds = await guildService.GetAllGuildsAsync(context);
        foreach (var guild in guilds.Value)
        {
            logger.Context(context).Information("Updating shop for Guild {Id}", guild.Id);
            try
            {
                await UpdateGuildShopAsync(guild, context);
            }
            catch (Exception e)
            {
                logger.Context(context).Error(e, "Failed to update shop");
            }
        }
    }

    public async Task UpdateGuildShopAsync(GuildReadModel guild, TransactionContext context)
    {
        var characters = await characterService.GetAllCharactersInGuild(new GuildId(guild.Id), context);
        var shop =
            await shopService.GetGuildShopAsync(new GuildId(guild.Id), context);

        foreach (var character in characters.Value)
        {
            logger.Context(context).Verbose("Updating character shop for character {Id}", character.Id);
            var equip = GetNewEquip(character);
            logger.Here().Verbose("New Equip: {@Equip}", equip);
            await shopService.UpdateShopInventoryAsync(new GuildId(guild.Id), new CharacterId(character.Id),
                new ShopId(shop.Value.Id), equip, context);
        }
    }

    private IEnumerable<Item> GetNewEquip(CharacterReadModel character)
    {
        var random = new Random();
        var level = character.Level.CurrentLevel;

        var selector = new DynamicWeightedRandomizer<int>();
        selector.Add(5, 6);
        selector.Add(6, 5);
        selector.Add(7, 4);
        selector.Add(8, 3);
        selector.Add(9, 2);
        selector.Add(10, 1);
        var numOfItems = selector.NextWithReplacement();

        for (int i = 0; i < numOfItems; i++)
        {
            var rarity = rarityGenerator.GenerateShopRarity();
            var aspect = aspectGenerator.GetRandomAspect(rarity);
            if (random.Next(3) == 0)
                yield return itemGenerator.GenerateRandomWeapon(rarity, level, aspect);
            else
                yield return itemGenerator.GenerateRandomEquipment(rarity, level, aspect);
        }
    }
}