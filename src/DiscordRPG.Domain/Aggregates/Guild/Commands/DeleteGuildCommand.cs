using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Guild.Commands;

public class DeleteGuildCommand : Command<GuildAggregate, GuildId>
{
    public DeleteGuildCommand(GuildId aggregateId) : base(aggregateId)
    {
    }
}

public class DeleteGuildCommandHandler : CommandHandler<GuildAggregate, GuildId, DeleteGuildCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, DeleteGuildCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete();
        return Task.CompletedTask;
    }
}