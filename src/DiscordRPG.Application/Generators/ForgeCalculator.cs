using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class ForgeCalculator
{
    public (int str, int vit, int agi, int intel) GetStatsFromIngredients(IEnumerable<(Item item, int amount)> items,
        int factor = 2)
    {
        var strengthWort = items.Where(i => i.item.DamageAttribute == CharacterAttribute.Strength)
            .Select(i => i.item.Worth * i.amount).Sum() / factor;
        var vitWorth = items.Where(i => i.item.DamageAttribute == CharacterAttribute.Vitality)
            .Select(i => i.item.Worth * i.amount).Sum() / factor;
        var agiWorth = items.Where(i => i.item.DamageAttribute == CharacterAttribute.Agility)
            .Select(i => i.item.Worth * i.amount).Sum() / factor;
        var intWorth = items.Where(i => i.item.DamageAttribute == CharacterAttribute.Intelligence)
            .Select(i => i.item.Worth * i.amount).Sum() / factor;

        return (GenerateStat(strengthWort), GenerateStat(vitWorth), GenerateStat(agiWorth), GenerateStat(intWorth));
    }

    public Rarity GetRarityFromIngredients(IEnumerable<(Item item, int amount)> items)
    {
        return (Rarity) (int) items.Average(i => (int) i.item.Rarity);
    }

    private static int GenerateStat(int availableWorth)
    {
        var totalStatPoints = availableWorth / 10;

        return totalStatPoints;
    }
}