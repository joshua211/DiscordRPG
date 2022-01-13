namespace DiscordRPG.Application.Worker;

public class ShopWorker
{
    /*private readonly IAspectGenerator aspectGenerator;
    private readonly ICharacterService characterService;
    private readonly IGuildService guildService;
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly IRarityGenerator rarityGenerator;
    private readonly IShopService shopService;

    public ShopWorker(IShopService shopService, IItemGenerator itemGenerator, ICharacterService characterService,
        IGuildService guildService, IRarityGenerator rarityGenerator, IAspectGenerator aspectGenerator, ILogger logger)
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
        var guilds = await guildService.GetAllGuildsAsync();
        foreach (var guild in guilds.Value)
        {
            logger.Information("Updating shop for Guild {Id}", guild.ID);
            await UpdateGuildShopAsync(guild);
        }
    }

    public async Task UpdateGuildShopAsync(Guild guild)
    {
        var characters = await characterService.GetAllCharactersInGuild(guild.ID);
        var shop =
            await shopService.GetGuildShopAsync(guild.ID);
        var tasks = new List<Task>();
        foreach (var character in characters.Value)
        {
            logger.Here().Debug("Updating character shop for character {Id}", character.ID);
            var equip = GetNewEquip(character);
            logger.Here().Verbose("New Equip: {@Equip}", equip);
            tasks.Add(shopService.UpdateWaresAsync(shop.Value, character, equip.ToList()));
        }

        await Task.WhenAll(tasks);
    }

    private IEnumerable<Equipment> GetNewEquip(Character character)
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
    }*/
}