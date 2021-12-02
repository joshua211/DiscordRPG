using DiscordRPG.Application.Interfaces.Generators;

namespace DiscordRPG.Application.Generators;

public class NameGenerator : GeneratorBase, INameGenerator
{
    private static readonly Dictionary<Rarity, List<string>> dungeonAdjectives;
    private static readonly Dictionary<Rarity, List<string>> dungeonNouns;

    static NameGenerator()
    {
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
}