using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Entities.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IItemGenerator
{
    IEnumerable<Item> GenerateItems(Character character, Dungeon dungeon);
    Item GetHealthPotion(Rarity rarity, uint level);
    Item GenerateRandomWeapon(Rarity rarity, uint level, Aspect aspect);
    Item GenerateRandomEquipment(Rarity rarity, uint level, Aspect aspect);
}