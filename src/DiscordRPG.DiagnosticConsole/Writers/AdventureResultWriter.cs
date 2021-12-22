using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class AdventureResultWriter
{
    public static void Write(AdventureResult result, Character character)
    {
        var table = new Table();
        table.AddColumn("Result");
        table.AddColumn("Items");
        table.AddColumn("Experience");
        table.AddColumn("Encounters");
        table.AddColumn("Wounds");

        var resultText = result.Wounds.Sum(w => w.DamageValue) > character.CurrentHealth ? "Died" : "Completed";

        var itemsTable = new Table();
        itemsTable.AddColumn("Name");
        itemsTable.AddColumn("Rarity");
        itemsTable.AddColumn("Worth");
        itemsTable.AddColumn("Type");
        foreach (var item in result.Items)
        {
            itemsTable.AddRow(item.Name, item.Rarity.ToString(), item.Worth.ToString(), item.GetType().Name);
        }

        var exp = new Panel(result.Experience.ToString());

        var encounterTable = new Table();
        encounterTable.AddColumn("Level");
        encounterTable.AddColumn("Damage");
        encounterTable.AddColumn("Damage Type");
        encounterTable.AddColumn("Health");
        encounterTable.AddColumn("Armor");
        encounterTable.AddColumn("Magic Armor");
        foreach (var encounter in result.Encounters)
        {
            encounterTable.AddRow(encounter.Level.ToString(), encounter.Damage.Value.ToString(),
                encounter.Damage.DamageType.ToString(), encounter.Health.ToString(), encounter.Armor.ToString(),
                encounter.MagicArmor.ToString());
        }

        var woundsTable = new Table();
        woundsTable.AddColumn("Damage");
        foreach (var w in result.Wounds)
        {
            woundsTable.AddRow(w.DamageValue.ToString());
        }

        table.AddRow(new Panel(resultText), itemsTable, exp, encounterTable, woundsTable);
        AnsiConsole.Write(table);
    }
}