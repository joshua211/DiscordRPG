namespace DiscordRPG.Application.Data;

public static class Equip
{
    public static Equipment StarterArmor => new Equipment("Leather Armor", "A basic leather armor", Rarity.Common, 5, 2,
        1, 1,
        1, 1, 1, 1, EquipmentCategory.Body);

    public static Equipment StarterLeg => new Equipment("Leather Pants", "Some basic leather pants", Rarity.Common, 4,
        2, 1, 1,
        1, 1, 1, 1, EquipmentCategory.Leg);

    public static Weapon StarterWeapon => new Weapon("Rusty Dagger", "An old and rusty dagger", Rarity.Common, 0, 0, 1,
        1, 1, 1, 1, 1, CharacterAttribute.Strength, DamageType.Physical, 7, EquipmentCategory.Dagger);
}