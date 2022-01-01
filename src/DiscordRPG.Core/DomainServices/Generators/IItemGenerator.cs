using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Generators;

public interface IItemGenerator
{
    IEnumerable<Item> GenerateItems(Dungeon dungeon);
    Weapon GenerateRandomWeapon(Rarity rarity, uint level, Aspect aspect);
    Equipment GenerateRandomEquipment(Rarity rarity, uint level, Aspect aspect);
    public Item GenerateRandomItem(Rarity rarity, uint level, int amount);
    Equipment GenerateEquipment(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category);
    Weapon GenerateWeapon(Rarity rarity, uint level, Aspect aspect, EquipmentCategory category);
    Item GetHealthPotion(Rarity rarity, uint level);
    Item GenerateFromRecipe(Recipe recipe);
}