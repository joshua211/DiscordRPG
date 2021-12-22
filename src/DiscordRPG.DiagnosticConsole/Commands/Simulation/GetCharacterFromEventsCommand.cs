using DiscordRPG.Core.Events;
using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GetCharacterFromEventsCommand : ICommand
{
    private readonly IEventImporter eventImporter;
    private readonly ConsoleState state;

    public GetCharacterFromEventsCommand(IEventImporter eventImporter, ConsoleState state)
    {
        this.eventImporter = eventImporter;
        this.state = state;
    }

    public static string Command => "ev characters";
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

            var adventureResultCalculateds = events.OfType<AdventureResultCalculated>();
            var characters = adventureResultCalculateds.Select(d => d.Character).GroupBy(c => c.ID)
                .Select(gr => gr.Last());

            CharacterWriter.Write(characters);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    "take",
                    "back"
                }));
            if (choice == "back")
                continue;

            var choices = characters.Select(d => d.ID.Value);
            var id = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).UseConverter(s =>
            {
                var character = characters.First(d => d.ID.Value == s);
                return $"{character.CharacterName} Lvl: {character.Level.CurrentLevel}";
            }));

            var character = characters.First(d => d.ID.Value == id);
            state.SelectedCharacter = character;
        } while (choice != "back" && choice != "take");
    }
}