using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Generators;

public class ItemGenerator : GeneratorBase, IItemGenerator
{
    private readonly INameGenerator nameGenerator;
    private IWorthCalculator worthCalculator;

    public ItemGenerator(INameGenerator nameGenerator, IWorthCalculator worthCalculator)
    {
        this.nameGenerator = nameGenerator;
        this.worthCalculator = worthCalculator;
    }

    public IEnumerable<Item> GenerateItems(Dungeon dungeon)
    {
        var items = new List<Item>();
        var totalNum = GetNumOfItems();
        var level = dungeon.DungeonLevel;

        var selectorBuilder = new DynamicRandomSelector<int>();
        selectorBuilder.Add(0, 5);
        selectorBuilder.Add(1, 1);
        selectorBuilder.Add(2, 1);
        var selector = selectorBuilder.Build();

        for (int i = 0; i < totalNum; i++)
        {
            var rarity = GetItemRarityFromDungeonRarity(dungeon.Rarity);
            var itemType = selector.SelectRandomItem();
            switch (itemType)
            {
                case 0:

                    var num = GetNumOfItems(true);
                    var genItem = GenerateRandomItem(rarity, level, num);
                    var existing = items.FirstOrDefault(i => i.GetItemCode() == genItem.GetItemCode());
                    if (existing is null)
                        items.Add(genItem);
                    else
                        existing.Amount += genItem.Amount;
                    break;
                case 1:
                    items.Add(GenerateRandomEquipment(rarity, level, dungeon.DungeonAspect));
                    break;
                case 2:
                    items.Add(GenerateRandomWeapon(rarity, level, dungeon.DungeonAspect));
                    break;
            }
        }

        return items;
    }

    public Weapon GenerateRandomWeapon(Rarity rarity, uint level, Aspect aspect)
    {
        var cat = GenerateRandomWeaponCategory();

        return GenerateWeapon(rarity, level, aspect, cat);
    }

    public Equipment GenerateRandomEquipment(Rarity rarity, uint level, Aspect aspect)
    {
        var category = GenerateRandomEquipmentCategory();
        return GenerateEquipment(rarity, level, aspect, category);
    }

    public Item GenerateRandomItem(Rarity rarity, uint level, int amount)
    {
        var roundedLevel = level.RoundOff();
        var (name, descr) = nameGenerator.GenerateRandomItemName(rarity);
        var worth = worthCalculator.GetItemWorth(rarity, roundedLevel);

        return new Item(name, descr, rarity, worth, roundedLevel, amount, false);
    }

    public Equipment GenerateEquipment(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category)
    {
        var totalWorth = worthCalculator.GetItemWorth(rarity, level);
        int statWorth;
        int armorWorth;
        if (category is EquipmentCategory.Amulet or EquipmentCategory.Ring)
        {
            statWorth = totalWorth;
            armorWorth = 0;
        }
        else
        {
            statWorth = (int) (totalWorth * 0.5f);
            armorWorth = (int) (totalWorth * 0.5f);
        }

        statWorth = statWorth < 10 ? 10 : statWorth;

        var (s, v, a, i, l) = GenerateRandomStats(statWorth);
        var (armor, marmor) = GenerateRandomArmor(armorWorth);
        var position = GetPositionFromCategory(category);
        var name = nameGenerator.GenerateRandomEquipmentName(rarity, category, aspect);

        return new Equipment(name, "", rarity, armor, marmor, s, v, a, i, l, totalWorth, category, position, level);
    }

    public Weapon GenerateWeapon(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category)
    {
        var totalWorth = worthCalculator.GetItemWorth(rarity, level);
        var statWorth = (int) (totalWorth * 0.3f);
        statWorth = statWorth < 1 ? 1 : statWorth;
        var (s, v, a, i, l) = GenerateRandomStats(statWorth);
        var dmgWorth = (int) (totalWorth * 0.7f);
        var dmgValue = GetDamageValue(dmgWorth);

        var name = nameGenerator.GenerateRandomEquipmentName(rarity, category, aspect);
        var charAttr = GetScalingAttribute(category);
        var dmgType = GetDamageType(category);

        return new Weapon(name, "", rarity, 0, 0, s, v, a, i, l, totalWorth, charAttr, dmgType, dmgValue,
            category,
            level);
    }

    public Item GetHealthPotion(Rarity rarity, uint level)
    {
        var name = nameGenerator.GenerateHealthPotionName(rarity, level);
        var worth = worthCalculator.GetItemWorth(rarity, level);
        return new Item(name,
            $"A potion that can restore  {Math.Round(level * 10 * (1 + (int) rarity * 0.2f))} health points", rarity,
            worth,
            level, 1, true);
    }

    public Item GenerateFromRecipe(Recipe recipe)
    {
        var aspect = new Aspect("", new[] {"Crafted"});
        return (int) recipe.EquipmentCategory.Value > 4
            ? GenerateWeapon(recipe.Rarity, recipe.Level, aspect, recipe.EquipmentCategory.Value)
            : GenerateEquipment(recipe.Rarity, recipe.Level, aspect, recipe.EquipmentCategory.Value);
    }

    private int GetDamageValue(int availableWorth) => availableWorth / 3;

    private DamageType GetDamageType(EquipmentCategory category) => category switch
    {
        EquipmentCategory.Sword => DamageType.Physical,
        EquipmentCategory.Bow => DamageType.Physical,
        EquipmentCategory.Dagger => DamageType.Physical,
        EquipmentCategory.Spear => DamageType.Physical,
        EquipmentCategory.Mace => DamageType.Physical,
        EquipmentCategory.Scepter => DamageType.Magical,
        EquipmentCategory.Staff => DamageType.Magical,
        EquipmentCategory.Wand => DamageType.Magical,
        EquipmentCategory.Flail => DamageType.Magical
    };

    private CharacterAttribute GetScalingAttribute(EquipmentCategory category) => category switch
    {
        EquipmentCategory.Sword => CharacterAttribute.Strength,
        EquipmentCategory.Bow => CharacterAttribute.Strength,
        EquipmentCategory.Dagger => CharacterAttribute.Strength,
        EquipmentCategory.Spear => CharacterAttribute.Strength,
        EquipmentCategory.Mace => CharacterAttribute.Strength,
        EquipmentCategory.Scepter => CharacterAttribute.Intelligence,
        EquipmentCategory.Staff => CharacterAttribute.Intelligence,
        EquipmentCategory.Wand => CharacterAttribute.Intelligence,
        EquipmentCategory.Flail => CharacterAttribute.Intelligence
    };

    private EquipmentCategory GenerateRandomEquipmentCategory()
    {
        var arr = Enum.GetValues<EquipmentCategory>();
        return arr[random.Next(5)];
    }

    private static EquipmentPosition GetPositionFromCategory(EquipmentCategory category) => category switch
    {
        EquipmentCategory.Amulet => EquipmentPosition.Amulet,
        EquipmentCategory.Armor => EquipmentPosition.Armor,
        EquipmentCategory.Helmet => EquipmentPosition.Helmet,
        EquipmentCategory.Pants => EquipmentPosition.Pants,
        EquipmentCategory.Ring => EquipmentPosition.Ring,
        _ => EquipmentPosition.Weapon
    };

    private EquipmentCategory GenerateRandomWeaponCategory()
    {
        var arr = Enum.GetValues<EquipmentCategory>();
        return arr[random.Next(5, 13)];
    }

    private (int armor, int marmor) GenerateRandomArmor(int availableWorth)
    {
        var actualWorth = availableWorth / 2;
        int armor = 0, marmor = 0;
        var selectorBuilder = new DynamicRandomSelector<int>();
        selectorBuilder.Add(1, 2);
        selectorBuilder.Add(2, 1);
        var selector = selectorBuilder.Build();

        for (int i = 0; i < actualWorth; i++)
        {
            if (selector.SelectRandomItem() == 1)
                armor++;
            else
                marmor++;
        }

        return new(armor, marmor);
    }

    private (int str, int vit, int agi, int intel, int lck) GenerateRandomStats(int availableWorth)
    {
        var totalStatPoints = availableWorth / 10;
        var str = 0;
        var vit = 0;
        var agi = 0;
        var intel = 0;
        var lck = 0;

        for (int i = 0; i < totalStatPoints; i++)
        {
            switch (random.Next(0, 5))
            {
                case 0:
                    str++;
                    break;
                case 1:
                    vit++;
                    break;
                case 2:
                    agi++;
                    break;
                case 3:
                    intel++;
                    break;
                case 4:
                    lck++;
                    break;
            }
        }

        return new(str, vit, agi, intel, lck);
    }

    private int GetNumOfItems(bool isItem = false)
    {
        var selector = new DynamicRandomSelector<int>();
        if (isItem)
        {
            selector.Add(1, 0.5f);
            selector.Add(2, 0.7f);
            selector.Add(3, 1f);
            selector.Add(4, 0.7f);
            selector.Add(5, 0.5f);
        }
        else
        {
            selector.Add(1, 1);
            selector.Add(2, 0.5f);
            selector.Add(3, 0.1f);
        }

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
                selector.Add(Rarity.Uncommon, 1);
                break;
            case Rarity.Rare:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 1);
                selector.Add(Rarity.Rare, 1);
                break;
            case Rarity.Unique:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 1);
                selector.Add(Rarity.Rare, 1);
                selector.Add(Rarity.Unique, 1);
                break;
            case Rarity.Legendary:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 1);
                selector.Add(Rarity.Rare, 1);
                selector.Add(Rarity.Unique, 1);
                selector.Add(Rarity.Legendary, 1);
                break;
            case Rarity.Mythic:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 1);
                selector.Add(Rarity.Rare, 1);
                selector.Add(Rarity.Unique, 1);
                selector.Add(Rarity.Legendary, 1);
                selector.Add(Rarity.Mythic, 1);
                break;
            case Rarity.Divine:
                selector.Add(Rarity.Common, 1);
                selector.Add(Rarity.Uncommon, 1);
                selector.Add(Rarity.Rare, 1);
                selector.Add(Rarity.Unique, 1);
                selector.Add(Rarity.Legendary, 1);
                selector.Add(Rarity.Mythic, 1);
                selector.Add(Rarity.Divine, 1);
                break;
        }

        return selector.Build().SelectRandomItem();
    }
}