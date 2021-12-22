using DiscordRPG.Core.Entities;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class DungeonWriter
{
    public static void Write(Dungeon dungeon)
    {
        var table = new Table();
        table.Title(dungeon.Name);
        table.AddColumn("Level");
        table.AddColumn("Explorations");
        table.AddRow(
            new Table().AddColumn("Level").AddColumn("Rarity")
                .AddRow(dungeon.DungeonLevel.ToString(), dungeon.Rarity.ToString()),
            new Panel(dungeon.ExplorationsLeft.ToString()));

        AnsiConsole.Write(table);
    }

    public static void Write(IEnumerable<Dungeon> dungeons)
    {
        var table = new Table();
        table.Title($"{dungeons.Count()} dungeons");
        table.AddColumn("Name");
        table.AddColumn("Rarity");
        table.AddColumn("Level");
        foreach (var d in dungeons)
        {
            table.AddRow(d.Name, d.Rarity.ToString(), d.DungeonLevel.ToString());
        }

        AnsiConsole.Write(table);
    }
}