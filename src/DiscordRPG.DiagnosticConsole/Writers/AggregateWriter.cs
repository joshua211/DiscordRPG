using DiscordRPG.Domain.Aggregates.Guild;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public class AggregateWriter
{
    public static void Write(GuildAggregate aggregate)
    {
        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Characters");
        table.AddColumn("Dungeons");

        var charTable = new Table();
        charTable.AddColumn("Name");
        charTable.AddColumn("Class");
        charTable.AddColumn("Level");
        foreach (var character in aggregate.Characters)
        {
            charTable.AddRow(character.Name.Value, $"{character.Race.Name} {character.Class.Name}",
                character.CharacterLevel.CurrentLevel.ToString());
        }

        var dungeonTable = new Table();
        dungeonTable.AddColumn("Name");
        dungeonTable.AddColumn("Rarity");
        dungeonTable.AddColumn("Level");
        foreach (var dungeon in aggregate.Dungeons)
        {
            dungeonTable.AddRow(dungeon.Name.Value, dungeon.Rarity.ToString(), dungeon.Level.ToString());
        }

        table.AddRow(new Panel(aggregate.GuildName), charTable, dungeonTable);
        AnsiConsole.Write(table);
    }
}