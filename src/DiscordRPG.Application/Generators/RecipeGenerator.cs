using DiscordRPG.Application.Data;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class RecipeGenerator : GeneratorBase
{
    private readonly IItemGenerator itemGenerator;
    private readonly NameGenerator nameGenerator;

    public RecipeGenerator(IItemGenerator itemGenerator, NameGenerator nameGenerator)
    {
        this.itemGenerator = itemGenerator;
        this.nameGenerator = nameGenerator;
    }

    public IEnumerable<Recipe> GenerateRecipesForLevel(uint level)
    {
        foreach (var rarity in Enum.GetValues<Rarity>())
        {
            if (rarity == Rarity.Divine)
                yield break;

            yield return new Recipe(RecipeId.New, nameGenerator.GenerateHealthPotionName(rarity, level),
                $"A potion that can restore  {Math.Round(level * 20 * (1 + (int) rarity * 0.2f))} health points",
                rarity, level, RecipeCategory.HealthPotion, new List<Ingredient>
                {
                    new Ingredient(rarity, Items.ItemNamesByRarity[rarity][2].name, level, 5),
                    new Ingredient(rarity, Items.ItemNamesByRarity[rarity][3].name, level, 5)
                });
        }
    }
}