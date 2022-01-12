using DiscordRPG.Domain.Entities.Character.Enums;
using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class EquipmentInfo : ValueObject
{
    private readonly Dictionary<EquipmentPosition, Item> equipment;

    public EquipmentInfo(Item helmet, Item armor, Item pants, Item amulet, Item ring, Item weapon)
    {
        if (helmet.Position != EquipmentPosition.Helmet)
            DomainError.With(nameof(helmet));
        if (armor.Position != EquipmentPosition.Armor)
            DomainError.With(nameof(armor));
        if (pants.Position != EquipmentPosition.Pants)
            DomainError.With(nameof(pants));
        if (amulet.Position != EquipmentPosition.Amulet)
            DomainError.With(nameof(amulet));
        if (ring.Position != EquipmentPosition.Ring)
            DomainError.With(nameof(ring));
        if (weapon.Position != EquipmentPosition.Weapon)
            DomainError.With(nameof(weapon));

        Helmet = helmet;
        Armor = armor;
        Pants = pants;
        Amulet = amulet;
        Ring = ring;
        Weapon = weapon;

        equipment = new Dictionary<EquipmentPosition, Item>
        {
            {EquipmentPosition.Helmet, Helmet},
            {EquipmentPosition.Armor, Armor},
            {EquipmentPosition.Pants, Pants},
            {EquipmentPosition.Amulet, Amulet},
            {EquipmentPosition.Ring, Ring},
            {EquipmentPosition.Weapon, Weapon},
        };
    }

    public Item Helmet { get; private set; }
    public Item Armor { get; private set; }
    public Item Pants { get; private set; }
    public Item Amulet { get; private set; }
    public Item Ring { get; private set; }
    public Item Weapon { get; private set; }
}