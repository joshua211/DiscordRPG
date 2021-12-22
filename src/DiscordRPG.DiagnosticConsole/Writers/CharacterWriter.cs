using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.Entities;
using Serilog.Core;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class CharacterWriter
{
    private static IClassService classService = new Classes(Logger.None);
    private static IRaceService raceService = new Races(Logger.None);

    public static Table GetCharacterAsTable(Character character)
    {
        character.ClassService = classService;
        character.RaceService = raceService;
        var state = character.CurrentHealth <= 0 ? "Dead" : "Alive";

        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Level");
        table.AddColumn("Base");
        table.AddColumn("Health");
        table.AddColumn("Damage");
        table.AddColumn("Armor");
        table.AddColumn("Magic Armor");
        table.AddColumn("Stats");
        table.AddColumn("State");
        table.AddRow(
            new Markup(character.CharacterName),
            new Markup(character.Level.CurrentLevel.ToString()),
            new Table().AddColumn("Class").AddColumn("Race")
                .AddRow(character.CharacterClass.ClassName, character.CharacterRace.RaceName),
            new Table().AddColumn("Current").AddColumn("Max")
                .AddRow(character.CurrentHealth.ToString(), character.MaxHealth.ToString()),
            new Markup($"{character.TotalDamage.Value} {character.TotalDamage.DamageType}"),
            new Markup(character.Armor.ToString()),
            new Markup(character.MagicArmor.ToString()),
            new Table().AddColumn("STR").AddColumn("VIT").AddColumn("AGI").AddColumn("INT").AddColumn("LCK").AddRow(
                character.Stength.ToString(), character.Vitality.ToString(), character.Agility.ToString(),
                character.Intelligence.ToString(), character.Luck.ToString()),
            new Markup(state));

        return table;
    }

    public static void Write(Character character)
    {
        var table = GetCharacterAsTable(character);
        AnsiConsole.Write(table);
    }

    public static void Write(IEnumerable<Character> characters)
    {
        var table = new Table();
        table.Title($"{characters.Count()} characters");
        table.AddColumn("Name");
        table.AddColumn("Level");
        table.AddColumn("Class");
        table.AddColumn("Race");
        table.AddColumn("State");
        foreach (var c in characters)
        {
            c.ClassService = classService;
            c.RaceService = raceService;
            var state = c.CurrentHealth <= 0 ? "Dead" : "Alive";
            table.AddRow(c.CharacterName, c.Level.CurrentLevel.ToString(), c.CharacterClass.ClassName,
                c.CharacterRace.RaceName, state);
        }

        AnsiConsole.Write(table);
    }
}