using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IItemGenerator
{
    IEnumerable<Item> GenerateItems(Character character, Dungeon dungeon);
    Item GetHealthPotion(Rarity rarity, uint level);
}