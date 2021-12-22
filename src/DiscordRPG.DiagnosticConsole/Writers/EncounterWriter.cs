using DiscordRPG.Core.DomainServices;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class EncounterWriter
{
    public static void Write(Encounter encounter)
    {
        var table = new Table();
        table.Title("Encounter");
        table.AddColumn("Level");
        table.AddColumn("Damage");
        table.AddColumn("Damage Type");
        table.AddColumn("Health");
        table.AddColumn("Armor");
        table.AddColumn("Magic Armor");
        table.AddRow(encounter.Level.ToString(), encounter.Damage.Value.ToString(),
            encounter.Damage.DamageType.ToString(), encounter.Health.ToString(), encounter.Armor.ToString(),
            encounter.MagicArmor.ToString());

        AnsiConsole.Write(table);
    }
}