using DiscordRPG.Core.DomainServices;

namespace DiscordRPG.Application.Data;

public class Classes : IClassService
{
    private readonly Dictionary<int, Class> classes;
    private readonly ILogger logger;

    public Classes(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        classes = new Dictionary<int, Class>() {{1, Mage}, {2, Warrior}};
    }

    public static Class Mage => new Class("Mage")
    {
        Description = "You are harry, wizard!",
        BaseIntelligence = 18,
        BaseAgility = 6,
        BaseStrength = 6,
        BaseVitality = 10,
        BaseLuck = 10,
        AgilityModifier = 0.4f,
        IntelligenceModifier = 1.5f,
        LuckModifier = 0.5f,
        StrengthModifier = 0.4f,
        VitalityModifier = 0.4f
    };

    public static Class Warrior => new Class("Warrior")
    {
        Description = "The more aggressive kinda guy",
        BaseIntelligence = 5,
        BaseAgility = 7,
        BaseStrength = 16,
        BaseVitality = 12,
        BaseLuck = 10,
        AgilityModifier = 0.5f,
        IntelligenceModifier = 0.3f,
        LuckModifier = 0.5f,
        StrengthModifier = 1,
        VitalityModifier = 0.8f
    };

    public Class GetClass(int id)
    {
        if (!classes.TryGetValue(id, out var obj))
        {
            logger.Here().Error("No class with id {Id} found", id);
        }

        return obj;
    }

    public IEnumerable<(int id, Class @class)> GetAllClasses()
    {
        return classes.ToList().Select(kv => (kv.Key, kv.Value));
    }

    public IEnumerable<string> GetStrengths(Class charClass)
    {
        var list = new List<string>();
        if (charClass.AgilityModifier > 0.5f)
            list.Add("Agility");
        if (charClass.IntelligenceModifier > 0.5f)
            list.Add("Intelligence");
        if (charClass.LuckModifier > 0.5f)
            list.Add("Luck");
        if (charClass.StrengthModifier > 0.5f)
            list.Add("Strength");
        if (charClass.VitalityModifier > 0.5f)
            list.Add("Vitality");

        if (!list.Any())
            list.Add("None");

        return list;
    }

    public IEnumerable<string> GetWeaknesses(Class charClass)
    {
        var list = new List<string>();
        if (charClass.AgilityModifier < 0.5f)
            list.Add("Agility");
        if (charClass.IntelligenceModifier < 0.5f)
            list.Add("Intelligence");
        if (charClass.LuckModifier < 0.5f)
            list.Add("Luck");
        if (charClass.StrengthModifier < 0.5f)
            list.Add("Strength");
        if (charClass.VitalityModifier < 0.5f)
            list.Add("Vitality");

        if (!list.Any())
            list.Add("None");

        return list;
    }
}