using DiscordRPG.Application.Data;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.DiagnosticConsole.Commands.Helper;

public class CharacterHelper
{
    private readonly IItemGenerator itemGenerator;

    public CharacterHelper(IItemGenerator itemGenerator)
    {
        this.itemGenerator = itemGenerator;
    }

    public Character GetCharacter(uint level, Rarity rarity)
    {
        var aspect = Aspects.DebugAspect;

        var helmet =
            itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Helmet);
        var armor = itemGenerator.GenerateEquipment(rarity, level, aspect,
            EquipmentCategory.Armor);
        var pants = itemGenerator.GenerateEquipment(rarity, level, aspect,
            EquipmentCategory.Pants);
        var amulet =
            itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Amulet);
        var ring = itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Ring);
        var weapon = itemGenerator.GenerateWeapon(rarity, level, aspect, EquipmentCategory.Sword);
        var equipInfo = new EquipmentInfo(weapon, helmet, armor, pants, amulet, ring);

        return new Character(new DiscordId("DBG"), new Identity("DBG"), "DEBUG CHAR", 1,
            1,
            new Level(level, 0, 0), equipInfo,
            new List<Item>(),
            new List<Wound>(), 0);
    }
}