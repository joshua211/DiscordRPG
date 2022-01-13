using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Application.Data;

public static class Races
{
    private static readonly Dictionary<string, CharacterRace> races = new Dictionary<string, CharacterRace>()
    {
        {Human.Name, Human},
        {Dwarf.Name, Dwarf},
        {Elf.Name, Elf},
        {Mias.Name, Mias}
    };

    public static CharacterRace Human => new CharacterRace("Human",
        "The most mundane of all races, only luck brought them this far",
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f);

    public static CharacterRace Dwarf => new CharacterRace("Dwarf",
        "Another classic one",
        1f, 1, 0.2f, 0.3f, 0.5f);

    public static CharacterRace Elf => new CharacterRace("Elf",
        "The most ancient and generic fantasy race",
        0.5f, 0.5f, 0.7f, 0.8f, 0.5f);

    public static CharacterRace Mias => new CharacterRace("Mi'As",
        "Legends say the founder of this race did something legendary once, who knows",
        0.3f, 1f, 0.4f, 0.5f, 1f);

    public static CharacterRace GetRace(string name)
    {
        races.TryGetValue(name, out var c);
        return c;
    }

    public static IEnumerable<string> GetStrengths(CharacterRace race)
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

    public static IEnumerable<string> GetWeaknesses(CharacterRace race)
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