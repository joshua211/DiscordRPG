using DiscordRPG.Application.Data;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class NameGenerator : GeneratorBase
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
                    "Cave", "Forest", "Village"
                }
            },
            {
                Rarity.Uncommon, new List<string>()
                {
                    "Cave", "Forest", "Keep", "Hill", "Sect"
                }
            },
            {
                Rarity.Rare, new List<string>()
                {
                    "Cave", "Forest", "Castle", "Labyrinth"
                }
            },
            {
                Rarity.Unique, new List<string>()
                {
                    "Cave", "Forest", "Town", "Desert", "Jungle", "Peak", "Mountain"
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

    public (string name, string descr, CharacterAttribute attribute) GenerateRandomItemName(Rarity rarity)
    {
        var byRarity = Items.ItemNamesByRarity[rarity];
        return byRarity[random.Next(byRarity.Count)];
    }

    public string GenerateRandomEquipmentName(Rarity rarity, EquipmentCategory category, Aspect aspect)
    {
        var prefix = aspect.ItemPrefixes[rarity].ToList()[random.Next(aspect.ItemPrefixes[rarity].Count())];
        var noun = category.ToString();

        return $"{prefix} {noun}";
    }

    public string GenerateHealthPotionName(Rarity rarity, uint level)
    {
        var roman = rarity switch
        {
            Rarity.Common => "I",
            Rarity.Uncommon => "II",
            Rarity.Rare => "III",
            Rarity.Unique => "IV",
            Rarity.Legendary => "V",
            Rarity.Mythic => "VI",
            Rarity.Divine => "VII",
        };
        return $"Health Potion {roman} (Lvl. {level})";
    }
}