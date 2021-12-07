using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class NameGenerator : GeneratorBase, INameGenerator
{
    private static readonly Dictionary<Rarity, List<string>> dungeonAdjectives;
    private static readonly Dictionary<Rarity, List<string>> dungeonNouns;
    private static List<string> woundAdjectives;
    private static List<string> woundNouns;
    private static List<string> items;
    private static List<string> equipAdjectives;
    private static Dictionary<EquipmentCategory, string> equipmentNames;

    static NameGenerator()
    {
        equipmentNames = new Dictionary<EquipmentCategory, string>()
        {
            {EquipmentCategory.Amulet, "Amulet"},
            {EquipmentCategory.Body, "Armor"},
            {EquipmentCategory.Bow, "Bow"},
            {EquipmentCategory.Dagger, "Dagger"},
            {EquipmentCategory.Head, "Helmet"},
            {EquipmentCategory.Leg, "Leg Armor"},
            {EquipmentCategory.Mace, "Mace"},
            {EquipmentCategory.Ring, "Ring"},
            {EquipmentCategory.Scepter, "Scepter"},
            {EquipmentCategory.Spear, "Spear"},
            {EquipmentCategory.Staff, "Staff"},
            {EquipmentCategory.Sword, "Sword"}
        };
        equipAdjectives = new List<string>()
        {
            "Big", "Small", "Shiny", "Sharp", "Sparkling", "Flaming", "Freezing"
        };
        items = new List<string>()
        {
            "Hide",
            "Ore",
            "Pelt",
            "Bone",
        };
        woundAdjectives = new List<string>()
        {
            "Bleeding",
            "Broken",
            "Shattered",
            "Cut to the",
            "Bruised",
            "Destroyed",
            "Frozen",
            "Burned"
        };

        woundNouns = new List<string>()
        {
            "Skull",
            "Finger",
            "Foot",
            "Tooth",
            "Pinky",
            "Butt",
            "Torso",
            "Neck",
            "Fingernail",
            "Spine",
            "Bone",
            "Eyeball"
        };


        dungeonAdjectives = new Dictionary<Rarity, List<string>>
        {
            {
                Rarity.Common, new List<string>()
                {
                    "Dark", "Foul", "Big", "Small", "Dirty", "Gloomy", "Filthy", "Deep", "Underground", "Old", "Rusty",
                    "Gray", "Red", "Blue", "Green"
                }
            },
            {
                Rarity.Uncommon, new List<string>()
                {
                    "Creepy", "Vast", "Great", "Gloomy", "Old", "Cold", "Hot", "Dreary", "Solitary", "Gray", "Red",
                    "Blue", "Green"
                }
            },
            {
                Rarity.Rare, new List<string>()
                {
                    "Bloody", "Grand", "Ancient", "Cold", "Hot", "Silent", "Vile", "Sinister", "Gray", "Red", "Blue",
                    "Green"
                }
            },
            {
                Rarity.Unique, new List<string>()
                {
                    "Horrid", "Grand", "Ancient", "Illusionary", "Frightful", "Poisonous", "Cloudy", "Blazing",
                    "Freezing", "Gray", "Red", "Blue", "Green"
                }
            },
            {
                Rarity.Legendary, new List<string>()
                {
                    "Imperial", "Infernal", "Royal", "Infinite", "Ancestral"
                }
            },
            {
                Rarity.Mythic, new List<string>()
                {
                    "Hellish", "Eternal", "Icebound", "Everlasting", "Chaos"
                }
            },
            {
                Rarity.Divine, new List<string>()
                {
                    "Divine", "Celestial", "Godly"
                }
            }
        };

        dungeonNouns = new Dictionary<Rarity, List<string>>
        {
            {
                Rarity.Common, new List<string>()
                {
                    "Cave", "Forrest", "Village"
                }
            },
            {
                Rarity.Uncommon, new List<string>()
                {
                    "Cave", "Forrest", "Keep", "Hill", "Sect"
                }
            },
            {
                Rarity.Rare, new List<string>()
                {
                    "Cave", "Forrest", "Castle", "Labyrinth"
                }
            },
            {
                Rarity.Unique, new List<string>()
                {
                    "Cave", "Forrest", "Town", "Desert", "Jungle", "Peak", "Mountain"
                }
            },
            {
                Rarity.Legendary, new List<string>()
                {
                    "Palace", "Castle", "Tower", "Mountain", "Seal"
                }
            },
            {
                Rarity.Mythic, new List<string>()
                {
                    "Abyss", "Cavern", "Rift", "Palace"
                }
            },
            {
                Rarity.Divine, new List<string>()
                {
                    "Temple", "Rift", "Palace", "Abyss"
                }
            }
        };
    }

    public string GenerateDungeonName(Rarity rarity)
    {
        var adjectiveCategory = dungeonAdjectives[rarity];
        var adjective = adjectiveCategory[random.Next(0, adjectiveCategory.Count)];

        var nounCategory = dungeonNouns[rarity];
        var noun = nounCategory[random.Next(0, nounCategory.Count)];

        return $"{adjective} {noun}";
    }

    public string GenerateWoundName() =>
        $"{woundAdjectives[random.Next(woundAdjectives.Count)]} {woundNouns[random.Next(woundNouns.Count)]}";

    public string GenerateRandomItemName() => items[random.Next(items.Count)];

    public string GenerateRandomEquipmentName(Rarity rarity, EquipmentCategory category)
    {
        var adj = equipAdjectives[random.Next(equipAdjectives.Count)];
        var noun = equipmentNames[category];

        return $"{adj} {noun}";
    }

    public string GenerateRandomWeaponName(Rarity rarity, EquipmentCategory cat)
    {
        var adj = equipAdjectives[random.Next(equipAdjectives.Count)];
        var noun = equipmentNames[cat];

        return $"{adj} {noun}";
    }
}