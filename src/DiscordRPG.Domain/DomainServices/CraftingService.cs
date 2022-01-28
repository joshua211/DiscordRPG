using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Exceptions;

namespace DiscordRPG.Domain.DomainServices;

public class CraftingService : ICraftingService
{
    private readonly IItemGenerator itemGenerator;

    public CraftingService(IItemGenerator itemGenerator)
    {
        this.itemGenerator = itemGenerator;
    }

    public void CraftItem(GuildAggregate aggregate, CharacterId characterId, RecipeId recipeId,
        TransactionContext context)
    {
        var character = aggregate.Characters.FirstOrDefault(c => Equals(c.Id, characterId));
        var recipe = character.KnownRecipes.FirstOrDefault(r => r.Id == recipeId);
        if (!recipe.IsCraftableWith(character.Inventory))
            throw DomainError.With("Recipe is not craftable with the current inventory!");

        var newInventory = character.Inventory.ToList();
        switch (recipe.Category)
        {
            case RecipeCategory.HealthPotion:
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    var item = newInventory.FirstOrDefault(i =>
                        i.Name == ingredient.Name && i.Level == ingredient.Level && i.Rarity == ingredient.Rarity);
                    if (item.Amount > ingredient.Amount)
                        item.IncreaseAmount(-ingredient.Amount);
                    else if (item.Amount == ingredient.Amount)
                        newInventory.Remove(item);
                    else
                        throw DomainError.With($"Not enough {ingredient.Name} for that item");
                }

                var potion = itemGenerator.GetHealthPotion(recipe.Rarity, recipe.Level);
                var existing = newInventory.FirstOrDefault(i => i == potion);
                if (existing is not null)
                    existing.IncreaseAmount(1);
                else
                    newInventory.Add(potion);

                aggregate.ChangeCharacterInventory(characterId, newInventory, context);
                break;
            }
        }
    }
}

public interface ICraftingService
{
    void CraftItem(GuildAggregate aggregate, CharacterId characterId, RecipeId recipeId, TransactionContext context);
}