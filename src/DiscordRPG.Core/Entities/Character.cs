using DiscordRPG.Core.DomainServices;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.Entities;

public class Character : Entity
{
    private IClassService classService;

    private IRaceService raceService;

    public Character(DiscordId userId, Identity guildId, string characterName, int classId, int raceId,
        Level level,
        EquipmentInfo equipment, List<Item> inventory, List<Wound> wounds, int money)
    {
        UserId = userId;
        GuildId = guildId;
        CharacterName = characterName;
        ClassId = classId;
        RaceId = raceId;
        Level = level;
        Equipment = equipment;
        Inventory = inventory;
        Wounds = wounds;
        Money = money;
    }

    public DiscordId UserId { get; private set; }
    public Identity GuildId { get; private set; }
    public string CharacterName { get; private set; }
    public int ClassId { get; private set; }
    public int RaceId { get; private set; }
    public Level Level { get; private set; }
    public EquipmentInfo Equipment { get; private set; }
    public int Money { get; private set; }
    public List<Item> Inventory { get; private set; }
    public List<Wound> Wounds { get; private set; }


    [BsonIgnore] public Class CharacterClass { get; set; }

    [BsonIgnore] public Race CharacterRace { get; set; }

    [BsonIgnore] public IClassService ClassService
    {
        get => classService;
        set
        {
            classService = value;
            CharacterClass = classService.GetClass(ClassId);
        }
    }

    [BsonIgnore] public IRaceService RaceService
    {
        get => raceService;
        set
        {
            raceService = value;
            CharacterRace = raceService.GetRace(RaceId);
        }
    }

    #region Attributes

    public int Stength
    {
        get
        {
            var totalMod = CharacterClass.StrengthModifier + CharacterRace.StrengthModifier;
            var charAttr = (int) (CharacterClass.BaseStrength + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Equipment.GetTotalAttribute(CharacterAttribute.Strength);

            return charAttr + equipAttr;
        }
    }

    public int Vitality
    {
        get
        {
            var totalMod = CharacterClass.VitalityModifier + CharacterRace.VitalityModifier;
            var charAttr = (int) (CharacterClass.BaseVitality + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Equipment.GetTotalAttribute(CharacterAttribute.Vitality);

            return charAttr + equipAttr;
        }
    }

    public int Agility
    {
        get
        {
            var totalMod = CharacterClass.AgilityModifier + CharacterRace.AgilityModifier;
            var charAttr = (int) (CharacterClass.BaseAgility + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Equipment.GetTotalAttribute(CharacterAttribute.Agility);

            return charAttr + equipAttr;
        }
    }

    public int Intelligence
    {
        get
        {
            var totalMod = CharacterClass.IntelligenceModifier + CharacterRace.IntelligenceModifier;
            var charAttr = (int) (CharacterClass.BaseIntelligence + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Equipment.GetTotalAttribute(CharacterAttribute.Intelligence);

            return charAttr + equipAttr;
        }
    }

    public int Luck
    {
        get
        {
            var totalMod = CharacterClass.LuckModifier + CharacterRace.LuckModifier;
            var charAttr = (int) (CharacterClass.BaseLuck + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Equipment.GetTotalAttribute(CharacterAttribute.Luck);

            return charAttr + equipAttr;
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
                CharacterAttribute.Strength => Stength + (int) (Vitality * 0.1) + (int) (Agility * 0.1) +
                                               (int) (Intelligence * 0.1),
                CharacterAttribute.Vitality => Vitality + (int) (Stength * 0.1) + (int) (Agility * 0.1) +
                                               (int) (Intelligence * 0.1),
                CharacterAttribute.Agility => Agility + (int) (Vitality * 0.1) + (int) (Stength * 0.1) +
                                              (int) (Intelligence * 0.1),
                CharacterAttribute.Intelligence => Intelligence + (int) (Vitality * 0.1) + (int) (Agility * 0.1) +
                                                   (int) (Stength * 0.1),
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