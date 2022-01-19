/*using DiscordRPG.Application.Data;
using DiscordRPG.Application.Data.Models;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character.Enums;
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

    public IEnumerable<Recipe> GetAllItemRecipes(RecipeCategory category, int maxLevel)
    {
        return GetHealthPotionRecipes(maxLevel);
    }
    
    private IEnumerable<Recipe> GetHealthPotionRecipes(int level)
    {
        for (uint i = 1; i <= level; i = (i + 10).RoundOff())
        {
            foreach (var rarity in Enum.GetValues<Rarity>())
            {
                if (rarity == Rarity.Divine)
                    yield break;

                var name = nameGenerator.GenerateHealthPotionName(rarity, i);
                yield return new Recipe(rarity, i, name, RecipeCategory.HealthPotion, new List<(Rarity, string, uint, int)>
                {
                    new (rarity,Items.ItemNamesByRarity[rarity][2].name, i, 5),
                    new (rarity,Items.ItemNamesByRarity[rarity][3].name, i, 5)
                });
            }
        }
    }

   
}*/

