using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class SimulationCommand : ICommand
{
    public static string Command => "simulation";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    GenerateCommand.Command,
                    EventCommand.Command,
                    DungeonClearingSimulation.Command,
                    "back"
                }));
            if (choice == "back")
                continue;

            var command = commands.First(c => c.CommandName == choice);
            await command.ExecuteAsync(commands);
        } while (choice != "back");
    }
}