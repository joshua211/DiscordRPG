using DiscordRPG.DiagnosticConsole.Commands.Logging;
using DiscordRPG.DiagnosticConsole.Commands.Simulation;
using DiscordRPG.DiagnosticConsole.Commands.Tools;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands;

public class EntryCommand : ICommand
{
    public static string Command => "entry";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Command")
                .AddChoices(new[]
                {
                    LogsCommand.Command,
                    SimulationCommand.Command,
                    FixInventoryCommand.Command,
                    "close"
                }));
            if (choice == "close")
                continue;

            var command = commands.FirstOrDefault(c => c.CommandName == choice);
            if (command is null)
                AnsiConsole.WriteLine($"No command with name {choice} found");

            await command.ExecuteAsync(commands);
        } while (choice != "close");
    }
}