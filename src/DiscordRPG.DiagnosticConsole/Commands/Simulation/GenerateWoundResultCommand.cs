using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateWoundResultCommand : ICommand
{
    private readonly ConsoleState state;
    private readonly IWoundGenerator woundGenerator;

    public GenerateWoundResultCommand(IWoundGenerator woundGenerator, ConsoleState state)
    {
        this.woundGenerator = woundGenerator;
        this.state = state;
    }

    public static string Command => "wound result";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            var wounds = woundGenerator.GenerateWounds(state.SelectedCharacter, state.SelectedEncounter);
            WoundWriter.Write(wounds, state.SelectedCharacter);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices("generate", "back"));
        } while (choice != "back");
    }
}