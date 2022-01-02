using DiscordRPG.Common.Extensions;

namespace DiscordRPG.Core.ValueObjects;

public class Recipe
{
    public Recipe(Rarity rarity, uint level, string name, string description, Item item,
        List<(string ingredientName, int amount)> ingredients, EquipmentCategory? equipmentCategory = null)
    {
        Rarity = rarity;
        Level = level;
        Name = name;
        Description = description;
        Ingredients = ingredients;
        EquipmentCategory = equipmentCategory;
        Item = item;
    }

    public Rarity Rarity { get; private set; }
    public uint Level { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<(string ingredientName, int amount)> Ingredients { get; private set; }
    public Item? Item { get; set; }
    public EquipmentCategory? EquipmentCategory { get; private set; }

    public bool IsCraftableWith(IEnumerable<Item> items)
    {
        foreach (var (name, amount) in Ingredients)
        {
            var existing = items.FirstOrDefault(i =>
                i.Name == name && i.Rarity == Rarity && i.Level.RoundOff() == Level.RoundOff());
            if (existing is null || existing.Amount < amount)
                return false;
        }

        return true;
    }
}