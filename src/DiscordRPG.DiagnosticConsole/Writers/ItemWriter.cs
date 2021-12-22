using DiscordRPG.Core.ValueObjects;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class ItemWriter
{
    public static void Write(Item item)
    {
        var table = new Table();
        table.Title(item.Name);
        table.AddColumn("Worth");
        table.AddRow(item.Worth.ToString());
        table.Caption(item.Description);

        AnsiConsole.Write(table);
    }

    public static void Write(Equipment equipment)
    {
        var table = new Table();
        table.Title(equipment.Name);
        table.Caption(equipment.Description);
        table.AddColumn("Rarity");
        table.AddColumn("Worth");
        table.AddColumn("Armor");
        table.AddColumn("Magic Armor");
        table.AddColumn("Stats Bonus");

        table.AddRow(new Table()
                .AddColumn("Level")
                .AddColumn("Rarity")
                .AddRow(equipment.Level.ToString(), equipment.Rarity.ToString()),
            new Panel(equipment.Worth.ToString()),
            new Panel(equipment.Armor.ToString()),
            new Panel(equipment.MagicArmor.ToString()),
            new Table()
                .AddColumn("STR")
                .AddColumn("VIT")
                .AddColumn("AGI")
                .AddColumn("INT")
                .AddColumn("LCK")
                .AddRow(equipment.Strength.ToString(), equipment.Vitality.ToString(),
                    equipment.Agility.ToString(), equipment.Intelligence.ToString(),
                    equipment.Luck.ToString())
        );

        AnsiConsole.Write(table);
    }

    public static void Write(Weapon weapon)
    {
        var table = new Table();
        table.Title(weapon.Name);
        table.Caption(weapon.Description);
        table.AddColumn("Rarity");
        table.AddColumn("Worth");
        table.AddColumn("Damage");
        table.AddColumn("Attribute");
        table.AddColumn("Stats Bonus");

        table.AddRow(new Table()
                .AddColumn("Level")
                .AddColumn("Rarity")
                .AddRow(weapon.Level.ToString(), weapon.Rarity.ToString()),
            new Panel(weapon.Worth.ToString()),
            new Table().AddColumn("Type").AddColumn("Value")
                .AddRow(weapon.DamageType.ToString(), weapon.DamageValue.ToString()),
            new Panel(weapon.DamageAttribute.ToString()),
            new Table()
                .AddColumn("STR")
                .AddColumn("VIT")
                .AddColumn("AGI")
                .AddColumn("INT")
                .AddColumn("LCK")
                .AddRow(weapon.Strength.ToString(), weapon.Vitality.ToString(),
                    weapon.Agility.ToString(), weapon.Intelligence.ToString(),
                    weapon.Luck.ToString())
        );

        AnsiConsole.Write(table);
    }

    public static void Write(IEnumerable<Item> items)
    {
        var table = new Table();
        table.AddColumn("Type");
        table.AddColumn("Name");
        table.AddColumn("Worth");
        table.AddColumn("Level");
        table.AddColumn("Rarity");
        table.AddColumn("Armor");
        table.AddColumn("MArmor");
        table.AddColumn("Damage");
        table.AddColumn("DamageType");
        table.AddColumn("Attribute");
        table.AddColumn("STR");
        table.AddColumn("VIT");
        table.AddColumn("AGI");
        table.AddColumn("INT");
        table.AddColumn("LCK");

        foreach (var item in items)
        {
            if (item is Item i and not Equipment)
                table.AddRow("Item", item.Name, item.Worth.ToString(), item.Level.ToString(), item.Rarity.ToString());
            if (item is Equipment e and not Weapon)
                table.AddRow("Equipment", item.Name, item.Worth.ToString(), item.Level.ToString(),
                    item.Rarity.ToString(), e.Armor.ToString(), e.MagicArmor.ToString(), "", "", "",
                    e.Strength.ToString(), e.Vitality.ToString(), e.Agility.ToString(), e.Intelligence.ToString(),
                    e.Luck.ToString());
            else if (item is Weapon w)
                table.AddRow("Weapon", item.Name, item.Worth.ToString(), item.Level.ToString(),
                    item.Rarity.ToString(), "", "", w.DamageValue.ToString(), w.DamageType.ToString(),
                    w.DamageAttribute.ToString(),
                    w.Strength.ToString(), w.Vitality.ToString(), w.Agility.ToString(), w.Intelligence.ToString(),
                    w.Luck.ToString());
        }

        AnsiConsole.Write(table);
    }
}