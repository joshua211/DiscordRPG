using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateEncounterResultCommand : ICommand
{
    private readonly IEncounterGenerator encounterGenerator;
    private readonly ConsoleState state;

    public GenerateEncounterResultCommand(IEncounterGenerator encounterGenerator, ConsoleState state)
    {
        this.encounterGenerator = encounterGenerator;
        this.state = state;
    }

    public static string Command => "encounter result";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            var encounter = encounterGenerator.CreateDungeonEncounter(state.SelectedDungeon);
            EncounterWriter.Write(encounter);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices("generate", "take", "back"));
            if (choice == "back")
                continue;

            state.SelectedEncounter = encounter;
        } while (choice != "back" && choice != "take");
    }
}