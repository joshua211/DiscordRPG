namespace DiscordRPG.Application.Worker;

public class DiagnosticsWorker
{
    /*private readonly IMongoCollection<DomainEvent> collection;
    private readonly ILogger logger;
    private readonly ConcurrentBag<DomainEvent> uncommittedEvents;

    public DiagnosticsWorker(IDatabaseSettings databaseSettings, ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
        var client = new MongoClient(databaseSettings.ConnectionString);
        collection = client.GetDatabase(databaseSettings.DiagnosticDatabaseName)
            .GetCollection<DomainEvent>(databaseSettings.DiagnosticEventCollectionName);
        uncommittedEvents = new ConcurrentBag<DomainEvent>();
    }

    public void AddDomainEvent(DomainEvent ev) => uncommittedEvents.Add(ev);

    public async Task FlushAsync()
    {
        if (!uncommittedEvents.Any())
        {
            logger.Here().Verbose("No uncommitted events, skipping flush");
            return;
        }

        var bulk = new List<WriteModel<DomainEvent>>();
        lock (uncommittedEvents)
        {
            logger.Here().Debug("Flushing diagnostics with {Count} events", uncommittedEvents.Count);
            bulk = uncommittedEvents.Select(ev => new InsertOneModel<DomainEvent>(ev)).Cast<WriteModel<DomainEvent>>()
                .ToList();
            uncommittedEvents.Clear();
        }

        var result = await collection.BulkWriteAsync(bulk);
        logger.Here().Debug("Saved {Count} entries", result.InsertedCount);
    }*/
}