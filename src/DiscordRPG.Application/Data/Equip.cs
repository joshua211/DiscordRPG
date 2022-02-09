using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Data;

public static class Equip
{
    public static Item StarterArmor => new Item(ItemId.New, "Leather Armor", "A basic leather armor", 1, Rarity.Common,
        EquipmentCategory.Armor, EquipmentPosition.Armor, ItemType.Equipment, CharacterAttribute.Agility,
        DamageType.Magical, null, 1, 1, 3, 2, 0, 1, 1, 0, 0, 0, true, UsageEffect.None);

    public static Item StarterLeg => new Item(ItemId.New, "Leather Pants", "Some basic leather pants", 1, Rarity.Common,
        EquipmentCategory.Armor, EquipmentPosition.Pants, ItemType.Equipment, CharacterAttribute.Agility,
        DamageType.Magical, null, 1, 1, 2, 1, 1, 0, 0, 0, 0, 0, true, UsageEffect.None);

    public static Item StarterWeapon => new Item(ItemId.New, "Rusty Dagger", "An old and rusty dagger",
        1,
        Rarity.Common,
        EquipmentCategory.Dagger, EquipmentPosition.Weapon, ItemType.Weapon, CharacterAttribute.Strength,
        DamageType.Physical, null, 1, 1, 0, 0, 1, 0, 0, 0, 0, 4, true, UsageEffect.None);

    public static Item StarterAmulet => new Item(ItemId.New, "Alpha Amulet", "The amulet of a true alpha",
        1,
        Rarity.Legendary,
        EquipmentCategory.Amulet, EquipmentPosition.Amulet, ItemType.Equipment, CharacterAttribute.Intelligence,
        DamageType.Physical, new StatusEffect(StatusEffectType.ExpBoost, 0.1f, "Alpha Blessing"), 1, 1, 0, 0, 0, 0, 0,
        0, 0, 0, true, UsageEffect.None);
}