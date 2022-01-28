using DiscordRPG.Application.Data;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class RecipeGenerator : GeneratorBase
{
    private readonly IHealthPotionCalculator calculator;
    private readonly IItemGenerator itemGenerator;
    private readonly NameGenerator nameGenerator;

    public RecipeGenerator(IItemGenerator itemGenerator, NameGenerator nameGenerator,
        IHealthPotionCalculator calculator)
    {
        this.itemGenerator = itemGenerator;
        this.nameGenerator = nameGenerator;
        this.calculator = calculator;
    }

    public IEnumerable<Recipe> GenerateRecipesForLevel(uint level)
    {
        foreach (var rarity in Enum.GetValues<Rarity>())
        {
            if (rarity == Rarity.Divine)
                yield break;

            yield return new Recipe(RecipeId.New, nameGenerator.GenerateHealthPotionName(rarity, level),
                $"A potion that can restore {calculator.CalculateHealAmount(rarity, level)} health points",
                rarity, level, RecipeCategory.HealthPotion, new List<Ingredient>
                {
                    new Ingredient(rarity, Items.ItemNamesByRarity[rarity][2].name, level, 5),
                    new Ingredient(rarity, Items.ItemNamesByRarity[rarity][3].name, level, 5)
                });
        }
    }
}