using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Generators;

public class ItemGenerator : GeneratorBase, IItemGenerator
{
    private readonly INameGenerator nameGenerator;

    public ItemGenerator(INameGenerator nameGenerator)
    {
        this.nameGenerator = nameGenerator;
    }

    public IEnumerable<Item> GenerateItems(Dungeon dungeon)
    {
        var numOfItems = GetNumOfItems();
        for (int j = 0; j < numOfItems; j++)
        {
            var rarity = GetItemRarityFromDungeonRarity(dungeon.Rarity);
            var level = dungeon.DungeonLevel;

            Item item = null;
            var equipType = random.Next(3);
            switch (equipType)
            {
                case 0:
                    item = GenerateRandomItem(rarity);
                    break;
                case 1:
                    item = GenerateRandomEquipment(rarity, level);
                    break;
                case 2:
                    item = GenerateRandomWeapon(rarity, level);
                    break;
            }

            yield return item;
        }
    }

    private Weapon GenerateRandomWeapon(Rarity rarity, uint level)
    {
        var str = GenerateRandomStat(rarity, level);
        var vit = GenerateRandomStat(rarity, level);
        var agi = GenerateRandomStat(rarity, level);
        var intel = GenerateRandomStat(rarity, level);
        var lck = GenerateRandomStat(rarity, level);
        var worth = GenerateRandomEquipmentWorth(rarity, str + vit + vit + agi + intel + lck);

        var cat = GenerateRandomWeaponCategory();

        return new Weapon(nameGenerator.GenerateRandomWeaponName(rarity, cat), "", rarity, 0, 0, str, vit, agi, intel,
            lck, worth, GenerateRandomCharAttribute(), GenerateRandomDamageType(),
            GenerateRandomDamageValue(rarity, level), cat);
    }

    private Equipment GenerateRandomEquipment(Rarity rarity, uint level)
    {
        var str = GenerateRandomStat(rarity, level);
        var vit = GenerateRandomStat(rarity, level);
        var agi = GenerateRandomStat(rarity, level);
        var intel = GenerateRandomStat(rarity, level);
        var lck = GenerateRandomStat(rarity, level);
        var armor = GenerateRandomArmor(rarity, level);
        var category = GenerateRandomEquipmentCategory();

        var randomArmor = random.Next(3);
        var worth = GenerateRandomEquipmentWorth(rarity, str + vit + vit + agi + intel + lck);
        switch (randomArmor)
        {
            case 0:
                return new Equipment(nameGenerator.GenerateRandomEquipmentName(rarity, category), "", rarity, armor, 0,
                    str, vit, agi, intel, lck, worth, category);
            case 1:
                return new Equipment(nameGenerator.GenerateRandomEquipmentName(rarity, category), "", rarity, armor / 2,
                    armor / 2, str, vit, agi, intel, lck, worth, category);
            case 2:
                return new Equipment(nameGenerator.GenerateRandomEquipmentName(rarity, category), "", rarity, 0, armor,
                    str, vit, agi, intel, lck, worth, category);
            default:
                return null;
        }
    }

    private Item GenerateRandomItem(Rarity rarity)
    {
        return new Item(nameGenerator.GenerateRandomItemName(), "", rarity, GenerateRandomItemWorth(rarity));
    }

    private int GenerateRandomDamageValue(Rarity rarity, uint level) =>
        (int) (13 * level * ((int) rarity + 1) * (random.NextDouble() + 0.1));

    private DamageType GenerateRandomDamageType() => Enum.GetValues<DamageType>()[random.Next(2)];

    private CharacterAttribute GenerateRandomCharAttribute() => Enum.GetValues<CharacterAttribute>()[random.Next(4)];

    private EquipmentCategory GenerateRandomEquipmentCategory()
    {
        var arr = Enum.GetValues<EquipmentCategory>();
        return arr[random.Next(5)];
    }

    private EquipmentCategory GenerateRandomWeaponCategory()
    {
        var arr = Enum.GetValues<EquipmentCategory>();
        return arr[random.Next(5, 11)];
    }

    private int GenerateRandomArmor(Rarity rarity, uint level) =>
        (int) (10 * level * ((int) rarity + 1) * (random.NextDouble() + 0.1));

    private int GenerateRandomStat(Rarity rarity, uint level) =>
        (int) (1 * level * ((int) rarity + 1) * (random.NextDouble() + 0.1));

    private int GenerateRandomEquipmentWorth(Rarity rarity, int totalStats) => 10 * totalStats * ((int) rarity + 1);

    private int GenerateRandomItemWorth(Rarity rarity) => random.Next(10, 1000) * ((int) rarity + 1);

    private int GetNumOfItems()
    {
        var selector = new DynamicRandomSelector<int>();
        selector.Add(1, 1);
        selector.Add(2, 0.5f);
        selector.Add(3, 0.1f);

        return selector.Build().SelectRandomItem();
    }

    private Rarity GetItemRarityFromDungeonRarity(Rarity rarity)
    {
        var selector = new DynamicRandomSelector<Rarity>();
        switch (rarity)
        {
            case Rarity.Common:
                selector.Add(Rarity.Common, 1);
                break;
            case Rarity.Uncommon:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                break;
            case Rarity.Rare:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                selector.Add(Rarity.Rare, 3);
                break;
            case Rarity.Unique:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                selector.Add(Rarity.Rare, 3);
                selector.Add(Rarity.Unique, 4);
                break;
            case Rarity.Legendary:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                selector.Add(Rarity.Rare, 3);
                selector.Add(Rarity.Unique, 4);
                selector.Add(Rarity.Legendary, 5);
                break;
            case Rarity.Mythic:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                selector.Add(Rarity.Rare, 3);
                selector.Add(Rarity.Unique, 4);
                selector.Add(Rarity.Legendary, 5);
                selector.Add(Rarity.Mythic, 6);
                break;
            case Rarity.Divine:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 2);
                selector.Add(Rarity.Rare, 3);
                selector.Add(Rarity.Unique, 4);
                selector.Add(Rarity.Legendary, 5);
                selector.Add(Rarity.Mythic, 6);
                selector.Add(Rarity.Divine, 7);
                break;
        }

        return selector.Build().SelectRandomItem();
    }
}