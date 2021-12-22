using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Settings;
using MongoDB.Driver;

namespace DiscordRPG.DiagnosticConsole.Importers;

public class LiveLogImporter : ILiveLogImporter
{
    private readonly IMongoCollection<LogEntry> collection;
    private List<LogEntry> logs;
    private bool shouldUpdate;
    private DateTime startTime;
    private Thread thread;

    public LiveLogImporter(IMongoClient mongoClient, IDatabaseSettings settings)
    {
        collection = mongoClient.GetDatabase(settings.DatabaseName).GetCollection<LogEntry>(settings.LogCollectionName);
        logs = new List<LogEntry>();
    }

    public event EventHandler<IEnumerable<LogEntry>> NewLogs;

    public void Start()
    {
        thread = new Thread(UpdateAsync);
        startTime = DateTime.UtcNow;
        logs = new List<LogEntry>();
        shouldUpdate = true;
        thread.Start();
    }

    public void Stop()
    {
        shouldUpdate = false;
    }

    private async void UpdateAsync()
    {
        while (shouldUpdate)
        {
            var entries = (await collection.FindAsync(l => l.Timestamp > startTime)).ToList();
            var newLogs = entries.Where(l => !logs.Contains(l));

            NewLogs?.Invoke(this, newLogs);
            logs.AddRange(newLogs);
            await Task.Delay(1000);
        }
    }
}

public interface ILiveLogImporter
{
    event EventHandler<IEnumerable<LogEntry>> NewLogs;
    void Start();
    void Stop();
}