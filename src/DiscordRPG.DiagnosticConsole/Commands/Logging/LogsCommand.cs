using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Logging;

public class LogsCommand : ICommand
{
    public static string Command => "logs";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Logs")
                .AddChoices(new[]
                {
                    LiveLogsCommand.Command,
                    HistoryLogCommand.Command,
                    "back"
                }));
            if (choice == "back")
                continue;

            var command = commands.First(c => c.CommandName == choice);
            await command.ExecuteAsync(commands);
        } while (choice != "back");
    }
}