using DiscordRPG.Domain.Aggregates.Character.ValueObjects;

namespace DiscordRPG.Application.Data;

public static class Classes
{
    private static readonly Dictionary<string, CharacterClass> classes = new Dictionary<string, CharacterClass>()
    {
        {Mage.Name, Mage},
        {Thief.Name, Thief},
        {Knight.Name, Knight},
        {Warrior.Name, Warrior}
    };

    public static CharacterClass Mage => new("Mage", "You are harry, wizard!",
        6, 10, 6, 18, 10,
        0.4f, 0.4f, 0.4f, 1.5f, 0.5f);

    public static CharacterClass Thief => new("Thief", "Not only stealing hearts",
        10, 10, 15, 10, 10,
        0.7f, 0.5f, 1.2f, 0.3f, 0.5f);

    public static CharacterClass Knight => new("Knight", "TODO: funny description",
        12, 16, 5, 7, 10,
        1f, 1.2f, 0.3f, 0.3f, 0.4f);

    public static CharacterClass Warrior => new("Warrior", "The more aggressive kinda guy",
        16, 12, 7, 5, 10,
        1f, 0.9f, 0.5f, 0.3f, 0.5f);

    public static CharacterClass GetClass(string name)
    {
        classes.TryGetValue(name, out var c);
        return c;
    }

    public static IEnumerable<string> GetStrengths(CharacterClass charClass)
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

    public static IEnumerable<string> GetWeaknesses(CharacterClass charClass)
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

    public static IEnumerable<CharacterClass> GetAllClasses() => classes.Values.ToList();
}