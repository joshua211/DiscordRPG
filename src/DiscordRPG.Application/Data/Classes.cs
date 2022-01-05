using DiscordRPG.Core.DomainServices;

namespace DiscordRPG.Application.Data;

public class Classes : IClassService
{
    private readonly Dictionary<int, Class> classes;
    private readonly ILogger logger;

    public Classes(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        classes = new Dictionary<int, Class>() {{1, Mage}, {2, Warrior}, {3, Knight}, {4, Thief}, {5, Lucky}};
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

    public static Class Lucky => new Class("Lucky Bastard")
    {
        Description = "How are you still alive",
        BaseIntelligence = 5,
        BaseAgility = 5,
        BaseStrength = 5,
        BaseVitality = 5,
        BaseLuck = 20,
        AgilityModifier = 0.3f,
        IntelligenceModifier = 0.3f,
        LuckModifier = 1.8f,
        StrengthModifier = 0.3f,
        VitalityModifier = 0.3f
    };

    public static Class Thief => new Class("Thief")
    {
        Description = "Not only stealing hearts",
        BaseIntelligence = 5,
        BaseAgility = 15,
        BaseStrength = 10,
        BaseVitality = 10,
        BaseLuck = 10,
        AgilityModifier = 1.2f,
        IntelligenceModifier = 0.3f,
        LuckModifier = 0.5f,
        StrengthModifier = 0.7f,
        VitalityModifier = 0.5f
    };

    public static Class Knight => new Class("Knight")
    {
        Description = "TODO lustige beschreibung",
        BaseIntelligence = 2,
        BaseAgility = 5,
        BaseStrength = 10,
        BaseVitality = 15,
        BaseLuck = 8,
        AgilityModifier = 0.3f,
        IntelligenceModifier = 0.3f,
        LuckModifier = 0.4f,
        StrengthModifier = 1f,
        VitalityModifier = 1.2f
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
        VitalityModifier = 0.9f
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