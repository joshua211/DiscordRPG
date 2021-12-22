using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Settings;
using MongoDB.Driver;

namespace DiscordRPG.DiagnosticConsole.Importers;

public class HistoryLogImporter : IHistoryLogImporter
{
    private readonly IMongoCollection<LogEntry> collection;

    public HistoryLogImporter(IMongoClient client, IDatabaseSettings settings)
    {
        collection = client.GetDatabase(settings.DatabaseName).GetCollection<LogEntry>(settings.LogCollectionName);
    }

    public async Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime start, DateTime end)
    {
        var logsInRange =
            (await collection.FindAsync(l => l.Timestamp >= start.ToLocalTime() && l.Timestamp <= end.ToLocalTime()))
            .ToList();

        return logsInRange;
    }
}

public interface IHistoryLogImporter
{
    Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime start, DateTime end);
}