﻿using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class RecipeGenerator : GeneratorBase
{
    private readonly IItemGenerator itemGenerator;
    private readonly INameGenerator nameGenerator;

    public RecipeGenerator(IItemGenerator itemGenerator, INameGenerator nameGenerator)
    {
        this.itemGenerator = itemGenerator;
        this.nameGenerator = nameGenerator;
    }

    public IEnumerable<Recipe> GetAllItemRecipes(uint maxLevel)
    {
        foreach (var rarity in Enum.GetValues<Rarity>())
        {
            if (rarity == Rarity.Divine)
                yield break;

            for (uint i = 1; i <= maxLevel; i++)
            {
                yield return new Recipe(rarity, i, nameGenerator.GenerateHealthPotionName(rarity, i),
                    $"A potion that can restore  {Math.Round(i * 10 * (1 + (int) rarity * 0.2f))} health points",
                    itemGenerator.GetHealthPotion(rarity, i),
                    new List<(string ingredientName, int amount)>()
                    {
                        (Items.ItemNamesByRarity[rarity][2].name, (int) rarity % 2 == 0 ? 10 : 20),
                        (Items.ItemNamesByRarity[rarity][3].name, (int) rarity % 2 == 0 ? 10 : 20)
                    });
            }
        }
    }

    public IEnumerable<Recipe> GetAllEquipmentRecipes(uint maxLevel)
    {
        foreach (var rarity in Enum.GetValues<Rarity>())
        {
            if (rarity == Rarity.Divine)
                yield break;

            foreach (var category in Enum.GetValues<EquipmentCategory>())
            {
                for (uint i = 1; i <= maxLevel; i++)
                {
                    yield return new Recipe(rarity, i,
                        $"[{rarity.ToString()}] Crafted {category.ToString()} (Lvl. {i})", "Crafted Equipment", null,
                        new List<(string ingredientName, int amount)>()
                        {
                            (Items.ItemNamesByRarity[rarity][0].name, 20),
                            (Items.ItemNamesByRarity[rarity][1].name, 20)
                        }, category);
                }
            }
        }
    }
}