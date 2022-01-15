using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Data;

public static class Equip
{
    public static Item StarterArmor => new Item(ItemId.New, "Leather Armor", "A basic leather armor", 1, Rarity.Common,
        EquipmentCategory.Armor, EquipmentPosition.Armor, ItemType.Equipment, CharacterAttribute.Agility,
        DamageType.Magical, 1, 1, 3, 2, 0, 1, 1, 0, 0, 0, true);

    public static Item StarterLeg => new Item(ItemId.New, "Leather Pants", "Some basic leather pants", 1, Rarity.Common,
        EquipmentCategory.Armor, EquipmentPosition.Armor, ItemType.Equipment, CharacterAttribute.Agility,
        DamageType.Magical, 1, 1, 2, 1, 1, 0, 0, 0, 0, 0, true);

    public static Item StarterWeapon => new Item(ItemId.New, "Rusty Dagger", "An old and rusty dagger",
        1,
        Rarity.Common,
        EquipmentCategory.Dagger, EquipmentPosition.Weapon, ItemType.Weapon, CharacterAttribute.Strength,
        DamageType.Physical, 1, 1, 0, 0, 1, 0, 0, 0, 0, 4, true);
}