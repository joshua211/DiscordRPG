using DiscordRPG.Core.Events;
using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GetDungeonsFromEventsCommand : ICommand
{
    private readonly IEventImporter eventImporter;
    private readonly ConsoleState state;

    public GetDungeonsFromEventsCommand(IEventImporter eventImporter, ConsoleState state)
    {
        this.eventImporter = eventImporter;
        this.state = state;
    }

    public static string Command => "ev dungeons";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            IEnumerable<object> events = null;
            AnsiConsole.Clear();
            await AnsiConsole.Status().StartAsync("Loading...",
                async context => { events = await eventImporter.GetEventsAsync(); });

            var dungeonCreatedEvents = events.OfType<DungeonCreated>();
            var dungeons = dungeonCreatedEvents.Select(d => d.Dungeon);

            DungeonWriter.Write(dungeons);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    "take",
                    "back"
                }));
            if (choice == "back")
                continue;

            var choices = dungeons.Select(d => d.ID.Value);
            var id = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).UseConverter(s =>
            {
                var dungeon = dungeons.First(d => d.ID.Value == s);
                return $"[[{dungeon.Rarity}]] {dungeon.Name} (Lvl:{dungeon.DungeonLevel})";
            }));

            var dungeon = dungeons.First(d => d.ID.Value == id);
            state.SelectedDungeon = dungeon;
        } while (choice != "back" && choice != "take");
    }
}