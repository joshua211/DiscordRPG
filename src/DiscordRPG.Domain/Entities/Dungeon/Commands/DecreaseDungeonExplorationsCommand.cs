using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class DecreaseDungeonExplorationsCommand : Command<GuildAggregate, GuildId>
{
    public DecreaseDungeonExplorationsCommand(GuildId aggregateId, DungeonId dungeonId, TransactionContext context) :
        base(aggregateId)
    {
        DungeonId = dungeonId;
        Context = context;
    }

    public DungeonId DungeonId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class
    DecreaseDungeonExplorationsCommandHandler : CommandHandler<GuildAggregate, GuildId,
        DecreaseDungeonExplorationsCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, DecreaseDungeonExplorationsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.DecreaseExplorations(command.DungeonId, command.Context);
        return Task.CompletedTask;
    }
}