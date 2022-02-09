using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Commands;

public class DecreaseDungeonExplorationsCommand : Command<DungeonAggregate, DungeonId>
{
    public DecreaseDungeonExplorationsCommand(DungeonId aggregateId, DungeonId dungeonId, TransactionContext context) :
        base(aggregateId)
    {
        DungeonId = dungeonId;
        Context = context;
    }

    public DungeonId DungeonId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class
    DecreaseDungeonExplorationsCommandHandler : CommandHandler<DungeonAggregate, DungeonId,
        DecreaseDungeonExplorationsCommand>
{
    public override Task ExecuteAsync(DungeonAggregate aggregate, DecreaseDungeonExplorationsCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.DecreaseExplorations(command.DungeonId, command.Context);
        return Task.CompletedTask;
    }
}