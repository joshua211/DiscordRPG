using DiscordRPG.Application.Models;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Entities.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Generators;

public class ItemGenerator : GeneratorBase, IItemGenerator
{
    private readonly NameGenerator nameGenerator;
    private IWorthCalculator worthCalculator;

    public ItemGenerator(NameGenerator nameGenerator, IWorthCalculator worthCalculator)
    {
        this.nameGenerator = nameGenerator;
        this.worthCalculator = worthCalculator;
    }

    public IEnumerable<Item> GenerateItems(Character characterEntity, Dungeon dungeonEntity)
    {
        var character = new CharacterReadModel
        {
            Class = characterEntity.Class,
            Inventory = characterEntity.Inventory,
            Level = characterEntity.CharacterLevel,
            Money = characterEntity.Money,
            Name = characterEntity.Name,
            Race = characterEntity.Race,
            Wounds = characterEntity.Wounds
        };
        var dungeon = new DungeonReadModel
        {
            Aspect = dungeonEntity.Aspect,
            Explorations = dungeonEntity.Explorations,
            Level = dungeonEntity.Level,
            Name = dungeonEntity.Name,
            Rarity = dungeonEntity.Rarity
        };

        var items = new List<Item>();
        var totalNum = GetNumOfItems();
        var level = dungeon.Level.Value;

        var selector = new DynamicWeightedRandomizer<int>();
        selector.Add(0, 5);
        selector.Add(1, 1);
        selector.Add(2, 1);

        for (int i = 0; i < totalNum; i++)
        {
            var rarity = GetItemRarityFromDungeon(character, dungeon);
            var itemType = selector.NextWithReplacement();
            switch (itemType)
            {
                case 0:

                    var num = GetNumOfItems(true);
                    var genItem = GenerateRandomItem(rarity, level, num);
                    var existing = items.FirstOrDefault(item => item == genItem);
                    if (existing is null)
                        items.Add(genItem);
                    else
                        existing.IncreaseAmount(genItem.Amount);
                    break;
                case 1:
                    items.Add(GenerateRandomEquipment(rarity, level, dungeon.Aspect));
                    break;
                case 2:
                    items.Add(GenerateRandomWeapon(rarity, level, dungeon.Aspect));
                    break;
            }
        }

        return items;
    }

    public Item GetHealthPotion(Rarity rarity, uint level)
    {
        //TODO heal amount auslagern
        var name = nameGenerator.GenerateHealthPotionName(rarity, level);
        var worth = worthCalculator.CalculateWorth(rarity, level);
        return new Item(ItemId.New, name,
            $"A potion that can restore {Math.Round(level * 20 * (1 + (int) rarity * 0.2f))} health points", 1, rarity,
            EquipmentCategory.Amulet, EquipmentPosition.Amulet, ItemType.Consumable, CharacterAttribute.Intelligence,
            DamageType.Magical, worth, level, 0, 0, 0, 0, 0, 0, 0, 0, false);
    }

    public Item GenerateRandomWeapon(Rarity rarity, uint level, Aspect aspect)
    {
        var cat = GenerateRandomWeaponCategory();

        return GenerateWeapon(rarity, level, aspect, cat);
    }

    public Item GenerateRandomEquipment(Rarity rarity, uint level, Aspect aspect)
    {
        var category = GenerateRandomEquipmentCategory();
        return GenerateEquipment(rarity, level, aspect, category);
    }

    public Item GenerateRandomItem(Rarity rarity, uint level, int amount)
    {
        var roundedLevel = level.RoundOff();
        var (name, descr) = nameGenerator.GenerateRandomItemName(rarity);
        var worth = worthCalculator.CalculateWorth(rarity, roundedLevel);

        return new Item(ItemId.New, name,
            descr, amount, rarity,
            EquipmentCategory.Amulet, EquipmentPosition.Amulet, ItemType.Item, CharacterAttribute.Vitality,
            DamageType.Physical, worth, level, 0, 0, 0, 0, 0, 0, 0, 0, false);
    }

    public Item GenerateEquipment(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category)
    {
        var totalWorth = worthCalculator.CalculateWorth(rarity, level);
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

        return new Item(ItemId.New, name,
            "", 1, rarity,
            category, position, ItemType.Weapon, CharacterAttribute.Vitality,
            DamageType.Physical, totalWorth, level, armor, marmor, s, v, a, i, l, 0, false);
    }

    public Item GenerateWeapon(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category)
    {
        var totalWorth = worthCalculator.CalculateWorth(rarity, level);
        var statWorth = (int) (totalWorth * 0.3f);
        statWorth = statWorth < 1 ? 1 : statWorth;
        var (s, v, a, i, l) = GenerateRandomStats(statWorth);
        var dmgWorth = (int) (totalWorth * 0.7f);
        var dmgValue = GetDamageValue(dmgWorth);

        var name = nameGenerator.GenerateRandomEquipmentName(rarity, category, aspect);
        var charAttr = GetScalingAttribute(category);
        var dmgType = GetDamageType(category);

        return new Item(ItemId.New, name,
            "", 1, rarity,
            category, EquipmentPosition.Weapon, ItemType.Weapon, charAttr,
            dmgType, totalWorth, level, 0, 0, s, v, a, i, l, dmgValue, false);
    }

    /*public Item GenerateFromRecipe(Recipe recipe)
    {
        var aspect = Aspects.CraftedAspect;
        return (int) recipe.EquipmentCategory.Value > 4
            ? GenerateWeapon(recipe.Rarity, recipe.Level, aspect, recipe.EquipmentCategory.Value)
            : GenerateEquipment(recipe.Rarity, recipe.Level, aspect, recipe.EquipmentCategory.Value);
    }*/

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
        EquipmentCategory.Bow => CharacterAttribute.Agility,
        EquipmentCategory.Dagger => CharacterAttribute.Agility,
        EquipmentCategory.Spear => CharacterAttribute.Strength,
        EquipmentCategory.Mace => CharacterAttribute.Strength,
        EquipmentCategory.Scepter => CharacterAttribute.Intelligence,
        EquipmentCategory.Staff => CharacterAttribute.Intelligence,
        EquipmentCategory.Wand => CharacterAttribute.Intelligence,
        EquipmentCategory.Flail => CharacterAttribute.Vitality
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
        var selector = new DynamicWeightedRandomizer<int>
        {
            {1, 1},
            {2, 1}
        };

        for (int i = 0; i < actualWorth; i++)
        {
            if (selector.NextWithReplacement() == 1)
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
        var selector = new DynamicWeightedRandomizer<int>();
        if (isItem)
        {
            selector.Add(3, 4);
            selector.Add(4, 5);
            selector.Add(5, 7);
            selector.Add(6, 10);
            selector.Add(7, 5);
            selector.Add(8, 4);
        }
        else
        {
            selector.Add(1, 10);
            selector.Add(2, 5);
            selector.Add(3, 1);
        }

        return selector.NextWithReplacement();
    }

    private Rarity GetItemRarityFromDungeon(CharacterReadModel character, DungeonReadModel dungeon)
    {
        var selector = new DynamicWeightedRandomizer<int>();
        switch (dungeon.Rarity)
        {
            case Rarity.Common:
                selector.Add((int) Rarity.Common, 1);
                break;
            case Rarity.Uncommon:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, character.Luck);
                break;
            case Rarity.Rare:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Rare, character.Luck);
                break;
            case Rarity.Unique:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Rare, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Unique, character.Luck);
                break;
            case Rarity.Legendary:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Rare, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Unique, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Legendary, character.Luck);
                break;
            case Rarity.Mythic:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Rare, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Unique, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Legendary, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Mythic, character.Luck);
                break;
            case Rarity.Divine:
                selector.Add((int) Rarity.Common, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Uncommon, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Rare, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Unique, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Legendary, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Mythic, (int) dungeon.Level.Value * 2);
                selector.Add((int) Rarity.Divine, character.Luck);
                break;
        }

        return (Rarity) selector.NextWithReplacement();
    }
}