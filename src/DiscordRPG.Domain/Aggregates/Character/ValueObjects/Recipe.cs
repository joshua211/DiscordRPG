using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Enums;
using EventFlow.Core;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Aggregates.Character.ValueObjects;

public class Recipe : Entity<RecipeId>
{
    public Recipe(RecipeId id, string name, string description, Rarity rarity, uint level, RecipeCategory category,
        List<Ingredient> ingredients) : base(id)
    {
        Name = name;
        Description = description;
        Rarity = rarity;
        Level = level;
        Category = category;
        Ingredients = ingredients;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Rarity Rarity { get; private set; }
    public uint Level { get; private set; }
    public RecipeCategory Category { get; private set; }
    public List<Ingredient> Ingredients { get; private set; }

    public bool IsCraftableWith(List<Item> availableItems)
    {
        foreach (var ingredient in Ingredients)
        {
            var existing = availableItems.FirstOrDefault(i =>
                i.Name == ingredient.Name && i.Level == ingredient.Level && i.Rarity == ingredient.Rarity &&
                i.Amount >= ingredient.Amount);
            if (existing is null)
                return false;
        }

        return true;
    }
}

public class RecipeId : Identity<RecipeId>
{
    public RecipeId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }
}