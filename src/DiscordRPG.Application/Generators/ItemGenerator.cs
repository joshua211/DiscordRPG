﻿using DiscordRPG.Application.Extensions;
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
        var totalNum = GetNumOfItems();
        var level = dungeon.DungeonLevel;

        var selectorBuilder = new DynamicRandomSelector<int>();
        selectorBuilder.Add(0, 3);
        selectorBuilder.Add(1, 1);
        selectorBuilder.Add(2, 1);
        var selector = selectorBuilder.Build();

        for (int i = 0; i < totalNum; i++)
        {
            var rarity = GetItemRarityFromDungeonRarity(dungeon.Rarity);
            var equipType = selector.SelectRandomItem();
            switch (equipType)
            {
                case 0:
                {
                    var num = GetNumOfItems(true);
                    for (int y = 0; y < num; y++)
                    {
                        yield return GenerateRandomItem(rarity, level);
                    }
                }
                    break;
                case 1:
                    yield return GenerateRandomEquipment(rarity, level, dungeon.DungeonAspect);

                    break;
                case 2:
                    yield return GenerateRandomWeapon(rarity, level, dungeon.DungeonAspect);

                    break;
            }
        }
    }

    private Weapon GenerateRandomWeapon(Rarity rarity, uint level, Aspect aspect)
    {
        var str = GenerateRandomStat(rarity, level);
        var vit = GenerateRandomStat(rarity, level);
        var agi = GenerateRandomStat(rarity, level);
        var intel = GenerateRandomStat(rarity, level);
        var lck = GenerateRandomStat(rarity, level);

        var cat = GenerateRandomWeaponCategory();
        var name = nameGenerator.GenerateRandomEquipmentName(rarity, cat, aspect);
        var charAttr = GenerateRandomCharAttribute();
        var dmgType = GenerateRandomDamageType();
        var dmgValue = GenerateRandomDamageValue(rarity, level);
        var worth = GenerateEquipmentWorth(rarity, level, str + vit + vit + agi + intel + lck + dmgValue);

        return new Weapon(name, "", rarity, 0, 0, str, vit, agi, intel, lck, worth, charAttr, dmgType, dmgValue, cat,
            level);
    }

    private Equipment GenerateRandomEquipment(Rarity rarity, uint level, Aspect aspect)
    {
        var str = GenerateRandomStat(rarity, level);
        var vit = GenerateRandomStat(rarity, level);
        var agi = GenerateRandomStat(rarity, level);
        var intel = GenerateRandomStat(rarity, level);
        var lck = GenerateRandomStat(rarity, level);
        var armor = GenerateRandomArmor(rarity, level);
        var category = GenerateRandomEquipmentCategory();
        var name = nameGenerator.GenerateRandomEquipmentName(rarity, category, aspect);
        var worth = GenerateEquipmentWorth(rarity, level, str + vit + vit + agi + intel + lck + armor);

        var randomArmor = random.Next(3);
        switch (randomArmor)
        {
            case 0:
                return new Equipment(name, "", rarity, armor, 0, str, vit, agi, intel, lck, worth, category, level);
            case 1:
                return new Equipment(name, "", rarity, armor / 2, armor / 2, str, vit, agi, intel, lck, worth, category,
                    level);
            case 2:
                return new Equipment(name, "", rarity, 0, armor, str, vit, agi, intel, lck, worth, category, level);
            default:
                return null;
        }
    }

    private Item GenerateRandomItem(Rarity rarity, uint level)
    {
        var roundedLevel = level.RoundOff();
        var name = nameGenerator.GenerateRandomItemName(rarity);
        var worth = GenerateItemWorth(rarity, roundedLevel);

        return new Item(name.name, name.descr, rarity, worth, roundedLevel);
    }

    private int GenerateItemWorth(Rarity rarity, uint level)
    {
        return (int) (((int) rarity + 1) * level * 10);
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

    private int GenerateEquipmentWorth(Rarity rarity, uint level, int totalStats) =>
        (int) (((int) rarity + 1) * level + (10 * totalStats));


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