using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Logging;

public class LiveLogsCommand : ICommand
{
    private readonly ILiveLogImporter logImporter;
    private List<string> levels;

    public LiveLogsCommand(ILiveLogImporter logImporter)
    {
        this.logImporter = logImporter;
        logImporter.NewLogs += HandleLogs;
    }

    public static string Command => "live";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        AnsiConsole.Clear();
        levels = new List<string> {"Verbose", "Debug", "Information", "Warning", "Error"};
        logImporter.Start();
        string choice;
        do
        {
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Live Logs")
                .AddChoices("level", "back"));

            if (choice == "back")
                continue;

            levels = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>().AddChoices("Verbose", "Debug", "Information", "Warning", "Error"));
        } while (choice != "back");

        logImporter.Stop();
    }

    private void HandleLogs(object sender, IEnumerable<LogEntry> logs)
    {
        foreach (var log in logs)
        {
            if (levels.Contains(log.Level))
                LogWriter.Write(log);
        }
    }
}