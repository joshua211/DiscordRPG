using DiscordRPG.DiagnosticConsole.Models;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Writers;

public static class LogWriter
{
    public static void Write(LogEntry logEntry)
    {
        var timestamp = $"[grey53]{logEntry.Timestamp}[/]";
        string level;
        switch (logEntry.Level)
        {
            case "Verbose":
                level = $"[grey78]{logEntry.Level}[/]";
                break;
            case "Debug":
                level = $"[silver]{logEntry.Level}[/]";
                break;
            case "Information":
                level = $"[darkgreen]{logEntry.Level}[/]";
                break;
            case "Warning":
                level = $"[yellow2]{logEntry.Level}[/]";
                break;
            case "Error":
                level = $"[orangered1]{logEntry.Level}[/]";
                break;
            default:
                level = $"[grey23]{logEntry.Level}[/]";
                break;
        }

        AnsiConsole.MarkupLine($"[[{timestamp} {level}]] " + Markup.Escape(logEntry.RenderedMessage));
        if (!string.IsNullOrEmpty(logEntry.Exception))
            AnsiConsole.WriteLine(logEntry.Exception);
    }
}