using DiscordRPG.Core.DomainServices;

namespace DiscordRPG.Application.Data;

public class Races : IRaceService
{
    private readonly ILogger logger;

    private readonly Dictionary<int, Race> races;

    public Races(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        ;
        races = new Dictionary<int, Race>() {{1, Human}, {2, Elf}, {3, Dwarf}};
    }

    public static Race Human => new Race("Human")
    {
        Description = "The most mundane of all races, only luck brought them this far",
        AgilityModifier = 0.5f,
        IntelligenceModifier = 0.5f,
        LuckModifier = 1f,
        StrengthModifier = 0.5f,
        VitalityModifier = 0.5f
    };

    public static Race Dwarf => new Race("Dwarf")
    {
        Description = "Another classic one",
        AgilityModifier = 0.2f,
        IntelligenceModifier = 0.3f,
        LuckModifier = 0.5f,
        StrengthModifier = 1f,
        VitalityModifier = 1f
    };

    public static Race Elf => new Race("Elf")
    {
        Description = "The most ancient and generic fantasy race",
        AgilityModifier = 0.7f,
        IntelligenceModifier = 0.8f,
        LuckModifier = 0.5f,
        StrengthModifier = 0.5f,
        VitalityModifier = 0.5f
    };

    public static Race Mias => new Race("Mi'as")
    {
        Description = "Legends say the founder of this race did something legendary once, who knows",
        AgilityModifier = 0.4f,
        IntelligenceModifier = 0.5f,
        LuckModifier = 1f,
        StrengthModifier = 0.3f,
        VitalityModifier = 1f
    };

    public Race GetRace(int id)
    {
        if (!races.TryGetValue(id, out var obj))
        {
            logger.Here().Error("No class with id {Id} found", id);
        }

        return obj;
    }

    public IEnumerable<(int id, Race race)> GetAllRaces()
    {
        return races.ToList().Select(kv => (kv.Key, kv.Value));
    }

    public IEnumerable<string> GetStrengths(Race race)
    {
        var list = new List<string>();
        if (race.AgilityModifier > 0.5f)
            list.Add("Agility");
        if (race.IntelligenceModifier > 0.5f)
            list.Add("Intelligence");
        if (race.LuckModifier > 0.5f)
            list.Add("Luck");
        if (race.StrengthModifier > 0.5f)
            list.Add("Strength");
        if (race.VitalityModifier > 0.5f)
            list.Add("Vitality");

        if (!list.Any())
            list.Add("None");

        return list;
    }

    public IEnumerable<string> GetWeaknesses(Race race)
    {
        var list = new List<string>();
        if (race.AgilityModifier < 0.5f)
            list.Add("Agility");
        if (race.IntelligenceModifier < 0.5f)
            list.Add("Intelligence");
        if (race.LuckModifier < 0.5f)
            list.Add("Luck");
        if (race.StrengthModifier < 0.5f)
            list.Add("Strength");
        if (race.VitalityModifier < 0.5f)
            list.Add("Vitality");

        if (!list.Any())
            list.Add("None");

        return list;
    }
}