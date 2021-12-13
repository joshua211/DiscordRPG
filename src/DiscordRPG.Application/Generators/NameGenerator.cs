using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class NameGenerator : GeneratorBase, INameGenerator
{
    private static readonly Dictionary<Rarity, List<string>> dungeonNouns;
    private static List<string> woundAdjectives;
    private static List<string> woundNouns;

    static NameGenerator()
    {
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
        var nounCategory = dungeonNouns[rarity];
        var noun = nounCategory[random.Next(0, nounCategory.Count)];

        return noun;
    }

    public string GenerateWoundName() =>
        $"{woundAdjectives[random.Next(woundAdjectives.Count)]} {woundNouns[random.Next(woundNouns.Count)]}";

    public (string name, string descr) GenerateRandomItemName(Rarity rarity)
    {
        var byRarity = Items.ItemNamesByRarity[rarity];
        return byRarity[random.Next(byRarity.Count)];
    }

    public string GenerateRandomEquipmentName(Rarity rarity, EquipmentCategory category, Aspect aspect)
    {
        var prefix = aspect.ItemPrefixes.ToList()[random.Next(aspect.ItemPrefixes.Count())];
        var noun = category.ToString();

        return $"{prefix} {noun}";
    }
}