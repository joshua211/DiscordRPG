using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Guild.Commands;

public class DeleteGuildCommand : Command<GuildAggregate, GuildId>
{
    public DeleteGuildCommand(GuildId aggregateId, TransactionContext context) : base(aggregateId)
    {
        Context = context;
    }

    public TransactionContext Context { get; private set; }
}

public class DeleteGuildCommandHandler : CommandHandler<GuildAggregate, GuildId, DeleteGuildCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, DeleteGuildCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete(command.Context);
        return Task.CompletedTask;
    }
}