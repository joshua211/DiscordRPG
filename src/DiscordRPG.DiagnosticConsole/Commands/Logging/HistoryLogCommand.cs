using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Logging;

public class HistoryLogCommand : ICommand
{
    private readonly IHistoryLogImporter logImporter;

    public HistoryLogCommand(IHistoryLogImporter logImporter)
    {
        this.logImporter = logImporter;
    }

    public static string Command => "history";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        var levels = new List<string> {"Verbose", "Debug", "Information", "Warning", "Error"};
        string searchTerm = "";
        var startDate = AnsiConsole.Prompt(new TextPrompt<DateTime>("From:").DefaultValue(DateTime.UtcNow));
        var endDate = AnsiConsole.Prompt(new TextPrompt<DateTime>("To:").DefaultValue(DateTime.UtcNow));

        IEnumerable<LogEntry> logs = null;
        await AnsiConsole.Status().StartAsync("Loading...",
            async context => { logs = await logImporter.GetLogsAsync(startDate, endDate); });
        string choice;

        do
        {
            AnsiConsole.Clear();
            var filteredLogs = logs.Where(l => levels.Contains(l.Level));
            if (!string.IsNullOrEmpty(searchTerm))
                filteredLogs = filteredLogs.Where(l => l.ContainsString(searchTerm));

            foreach (var l in filteredLogs)
                LogWriter.Write(l);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"Logs from {startDate} to {endDate}" +
                       (!string.IsNullOrEmpty(searchTerm) ? $": {searchTerm}" : ""))
                .AddChoices("level", "search", "back"));

            switch (choice)
            {
                case "back":
                    continue;
                case "level":
                    levels = AnsiConsole.Prompt(new MultiSelectionPrompt<string>().AddChoices("Verbose", "Debug",
                        "Information", "Warning", "Error"));
                    break;
                case "search":
                    searchTerm = AnsiConsole.Prompt(new TextPrompt<string>("search: ").AllowEmpty());
                    break;
                default:
                    AnsiConsole.WriteLine($"Command {choice} not found");
                    break;
            }
        } while (choice != "back");
    }
}