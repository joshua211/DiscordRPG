namespace DiscordRPG.Core.DomainServices.Generators;

public interface INameGenerator
{
    string GenerateDungeonName(Rarity rarity);
    string GenerateWoundName();
    string GenerateRandomItemName();
    string GenerateRandomEquipmentName(Rarity rarity, EquipmentCategory category);
    string GenerateRandomWeaponName(Rarity rarity, EquipmentCategory cat);
}