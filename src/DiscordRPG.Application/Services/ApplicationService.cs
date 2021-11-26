using System.Runtime.CompilerServices;
using DiscordRPG.Common;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public abstract class ApplicationService
{
    private readonly ILogger logger;
    private readonly IMediator mediator;

    protected ApplicationService(IMediator mediator, ILogger logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    protected TransactionContext TransactionBegin(TransactionContext parentContext = null,
        [CallerMemberName] string transactionName = "")
    {
        var id = GenerateNewTransactionId();
        var context = new TransactionContext(id, transactionName, logger, DateTime.Now);
        if (parentContext is not null)
        {
            parentContext.SetChild(context);
            context = parentContext;
        }

        logger.Information("{TransactionID} Begin transaction: {TransactionName}", context.Id, context.Transaction);

        return context;
    }

    protected void TransactionWarning(TransactionContext context, string content, params object[] properties)
    {
        var template = $"{context.Id} {content}";
        logger.Warning(template, properties);
    }

    protected void TransactionDebug(TransactionContext context, string content, params object[] properties)
    {
        logger.Debug("{ID} " + content, context.Id, properties);
    }

    protected void TransactionError(TransactionContext context, string content, params object[] properties)
    {
        context.HasFailed = true;
        logger.Error("{ID} " + content, context.Id, properties);
    }

    protected void TransactionError(TransactionContext context, Exception e)
    {
        context.HasFailed = true;
        logger.Error(e, "{ID} {Content}", context.Id, e.Message);
    }

    protected async Task<T> ProcessAsync<T>(TransactionContext context, Query<T> query,
        CancellationToken cancellationToken)
    {
        TransactionDebug(context, "Processing Query: {@Query}", query);

        return await mediator.Send(query, cancellationToken);
    }

    protected async Task<ExecutionResult> PublishAsync(TransactionContext context, Command command,
        CancellationToken cancellationToken)

    {
        TransactionDebug(context, "Publishing Command: {@Command}", command);

        return await mediator.Send(command, cancellationToken);
    }

    private static string GenerateNewTransactionId() => Guid.NewGuid().ToString().Split('-').Last();
}

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

    public void SetChild(TransactionContext ctx) => child = ctx;
}