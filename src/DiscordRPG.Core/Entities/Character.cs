namespace DiscordRPG.Core.Entities;

public class Character : Entity
{
    public Character(ulong userId, ulong guildId, string characterName, Class characterClass, Race characterRace,
        Level level,
        EquipmentInfo equipment, List<Item> inventory, List<Wound> wounds, int money)
    {
        UserId = userId;
        GuildId = guildId;
        CharacterName = characterName;
        CharacterClass = characterClass;
        CharacterRace = characterRace;
        Level = level;
        Equipment = equipment;
        Inventory = inventory;
        Wounds = wounds;
        Money = money;
    }

    public ulong UserId { get; private set; }
    public ulong GuildId { get; private set; }
    public string CharacterName { get; private set; }
    public Class CharacterClass { get; private set; }
    public Race CharacterRace { get; private set; }
    public Level Level { get; private set; }
    public EquipmentInfo Equipment { get; private set; }
    public int Money { get; private set; }
    public List<Item> Inventory { get; private set; }
    public List<Wound> Wounds { get; private set; }

    #region Attributes

    public int Stength
    {
        get
        {
            var totalMod = CharacterClass.StrengthModifier + CharacterRace.StrengthModifier;
            var charStrength = (int) ((CharacterClass.BaseStrength * Level.CurrentLevel) * totalMod);
            var equipStrength = Equipment.GetTotalAttribute(CharacterAttribute.Strength);

            return charStrength + equipStrength;
        }
    }

    public int Vitality
    {
        get
        {
            var totalMod = CharacterClass.VitalityModifier + CharacterRace.VitalityModifier;
            var charVit = (int) ((CharacterClass.BaseVitality * Level.CurrentLevel) * totalMod);
            var equipStrength = Equipment.GetTotalAttribute(CharacterAttribute.Vitality);

            return charVit + equipStrength;
        }
    }

    public int Agility
    {
        get
        {
            var totalMod = CharacterClass.AgilityModifier + CharacterRace.AgilityModifier;
            var charAg = (int) ((CharacterClass.BaseAgility * Level.CurrentLevel) * totalMod);
            var equipStrength = Equipment.GetTotalAttribute(CharacterAttribute.Agility);

            return charAg + equipStrength;
        }
    }

    public int Intelligence
    {
        get
        {
            var totalMod = CharacterClass.IntelligenceModifier + CharacterRace.IntelligenceModifier;
            var charInt = (int) ((CharacterClass.BaseIntelligence * Level.CurrentLevel) * totalMod);
            var equipStrength = Equipment.GetTotalAttribute(CharacterAttribute.Intelligence);

            return charInt + equipStrength;
        }
    }

    public int Luck
    {
        get
        {
            var totalMod = CharacterClass.LuckModifier + CharacterRace.LuckModifier;
            var charLuck = (int) ((CharacterClass.BaseLuck * Level.CurrentLevel) * totalMod);
            var equipStrength = Equipment.GetTotalAttribute(CharacterAttribute.Luck);

            return charLuck + equipStrength;
        }
    }

    public Damage TotalDamage
    {
        get
        {
            var weapon = Equipment.Weapon;
            if (weapon == null)
                return new Damage(DamageType.Physical, 1);

            var dmgFromAttributes = weapon.DamageAttribute switch
            {
                CharacterAttribute.Strength => Stength + (int) (Vitality * 0.7) + (int) (Agility * 0.7) +
                                               (int) (Intelligence * 0.7),
                CharacterAttribute.Vitality => Vitality + (int) (Stength * 0.7) + (int) (Agility * 0.7) +
                                               (int) (Intelligence * 0.7),
                CharacterAttribute.Agility => Agility + (int) (Vitality * 0.7) + (int) (Stength * 0.7) +
                                              (int) (Intelligence * 0.7),
                CharacterAttribute.Intelligence => Intelligence + (int) (Vitality * 0.7) + (int) (Agility * 0.7) +
                                                   (int) (Stength * 0.7),
                _ => 0
            };

            return new Damage(weapon.DamageType, dmgFromAttributes + weapon.DamageValue);
        }
    }

    public int MaxHealth => Vitality * 10;

    public int CurrentHealth => MaxHealth - Wounds.Sum(w => w.DamageValue);

    public int Armor => Equipment.GetTotalArmor();

    public int MagicArmor => Equipment.GetTotalMagicArmor();

    #endregion
}