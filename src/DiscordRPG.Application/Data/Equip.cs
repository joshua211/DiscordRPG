namespace DiscordRPG.Application.Data;

public static class Equip
{
    public static Equipment StarterArmor => new Equipment("Leather Armor", "A basic leather armor", Rarity.Common, 3, 2,
        0, 1,
        1, 0, 0, 1, EquipmentCategory.Armor, EquipmentPosition.Armor, 1);

    public static Equipment StarterLeg => new Equipment("Leather Pants", "Some basic leather pants", Rarity.Common, 2,
        2, 1, 0,
        0, 1, 1, 1, EquipmentCategory.Pants, EquipmentPosition.Pants, 1);

    public static Weapon StarterWeapon => new Weapon("Rusty Dagger", "An old and rusty dagger", Rarity.Common, 0, 0, 1,
        0, 0, 0, 0, 1, CharacterAttribute.Strength, DamageType.Physical, 4, EquipmentCategory.Dagger, 1);
}