using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.Enums;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateAdventureResultCommand : ICommand
{
    private readonly IAdventureResultService adventureResultService;
    private readonly ConsoleState state;

    public GenerateAdventureResultCommand(ConsoleState state, IAdventureResultService adventureResultService)
    {
        this.state = state;
        this.adventureResultService = adventureResultService;
    }

    public static string Command => "adventure result";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        AnsiConsole.Clear();
        do
        {
            var duration = AnsiConsole.Prompt(new SelectionPrompt<ActivityDuration>().AddChoices(ActivityDuration.Quick,
                ActivityDuration.Short, ActivityDuration.Medium, ActivityDuration.Long, ActivityDuration.ExtraLong));

            var result =
                adventureResultService.CalculateAdventureResult(state.SelectedCharacter, state.SelectedDungeon,
                    duration);
            AnsiConsole.Clear();
            AdventureResultWriter.Write(result, state.SelectedCharacter);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices("generate", "back"));
        } while (choice != "back");
    }
}