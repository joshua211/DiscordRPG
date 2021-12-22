using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class WoundWriter
{
    public static void Write(IEnumerable<Wound> wounds, Character character = null)
    {
        var table = new Table();
        table.Title($"{wounds.Count()} wounds");
        table.AddColumn("Description");
        table.AddColumn("Dmg");
        foreach (var w in wounds)
        {
            table.AddRow(w.Description, w.DamageValue.ToString());
        }

        var dmg = wounds.Sum(w => w.DamageValue);
        if (character is null)
            table.Caption($"{dmg} dmg");
        else
            table.Caption($"{dmg} dmg to {character.CurrentHealth} hp");


        AnsiConsole.Write(table);
    }
}