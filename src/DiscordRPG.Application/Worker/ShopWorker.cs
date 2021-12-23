using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Worker;

public class ShopWorker
{
    private readonly IAspectGenerator aspectGenerator;
    private readonly ICharacterService characterService;
    private readonly IGuildService guildService;
    private readonly IItemGenerator itemGenerator;
    private readonly IRarityGenerator rarityGenerator;
    private readonly IShopService shopService;

    public ShopWorker(IShopService shopService, IItemGenerator itemGenerator, ICharacterService characterService,
        IGuildService guildService, IRarityGenerator rarityGenerator, IAspectGenerator aspectGenerator)
    {
        this.shopService = shopService;
        this.itemGenerator = itemGenerator;
        this.characterService = characterService;
        this.guildService = guildService;
        this.rarityGenerator = rarityGenerator;
        this.aspectGenerator = aspectGenerator;
    }

    public async Task UpdateShopsAsync()
    {
        var guilds = await guildService.GetAllGuildsAsync();
        foreach (var guild in guilds.Value)
        {
            var characters = await characterService.GetAllCharactersInGuild(guild.ID);
            var shop =
                await shopService.GetGuildShopAsync(guild.ID);
            var tasks = new List<Task>();
            foreach (var character in characters.Value)
            {
                var equip = GetNewEquip(character);
                tasks.Add(shopService.UpdateWaresAsync(shop.Value.ID, character.ID, equip.ToList()));
            }

            await Task.WhenAll(tasks);
        }
    }

    private IEnumerable<Equipment> GetNewEquip(Character character)
    {
        var random = new Random();
        var level = character.Level.CurrentLevel;
        var rarity = rarityGenerator.GenerateShopRarity();

        var selector = new DynamicRandomSelector<int>();
        selector.Add(5, 6);
        selector.Add(6, 5);
        selector.Add(7, 4);
        selector.Add(8, 3);
        selector.Add(9, 2);
        selector.Add(10, 1);
        var numOfItems = selector.Build().SelectRandomItem();

        for (int i = 0; i < numOfItems; i++)
        {
            var aspect = aspectGenerator.GetRandomAspect(rarity);
            if (random.Next(3) == 0)
                yield return itemGenerator.GenerateRandomWeapon(rarity, level, aspect);
            else
                yield return itemGenerator.GenerateRandomEquipment(rarity, level, aspect);
        }
    }
}