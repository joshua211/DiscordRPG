namespace DiscordRPG.Core.DomainServices.Generators;

public interface INameGenerator
{
    string GenerateDungeonName(Rarity rarity);
    string GenerateWoundName();
    (string name, string descr) GenerateRandomItemName(Rarity rarity);
    string GenerateRandomEquipmentName(Rarity rarity, EquipmentCategory category, Aspect aspect);
}