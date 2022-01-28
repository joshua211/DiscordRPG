using System.Runtime.CompilerServices;
using Serilog;

namespace DiscordRPG.Common;

public class TransactionContext : IDisposable
{
    private readonly string id;
    private readonly ILogger logger;
    private readonly string transaction;
    private readonly DateTime transactionStart;
    private TransactionContext child;

    public bool HasFailed;

    public TransactionContext(string id, string transaction, ILogger logger, DateTime transactionStart,
        TransactionContext child = null)
    {
        this.id = id;
        this.transaction = transaction;
        this.logger = logger;
        this.transactionStart = transactionStart;
        this.child = child;
    }

    public string Id => child is null ? $"[{id}]" : $"[{id}]" + child.Id;

    public string Transaction => child is null ? transaction : child.Transaction;

    public void Dispose()
    {
        var time = DateTime.Now - transactionStart;

        var text = HasFailed ? "Unsuccessfully" : "Successfully";
        logger.Information("{ID} {Text} Completed Transaction: {Transaction} in {MS}ms", Id, text, Transaction,
            time.Milliseconds);
        child = null;
    }

    public IEnumerable<KeyValuePair<string, string>> AsMetadata() => new List<KeyValuePair<string, string>>
    {
        {new KeyValuePair<string, string>("transaction-id", id)},
    };

    public void SetChild(TransactionContext ctx) => child = ctx;

    public static TransactionContext New([CallerMemberName] string transactionName = "") =>
        new TransactionContext(GenerateNewTransactionId(), transactionName, Log.Logger, DateTime.UtcNow);

    public static TransactionContext With(string id, [CallerMemberName] string transactionName = "")
    {
        return new TransactionContext(id, transactionName, Log.Logger, DateTime.UtcNow);
    }

    private static string GenerateNewTransactionId() => Guid.NewGuid().ToString().Split('-').Last();
}