using DiscordRPG.Common;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Commands;

public class RemoveDungeonCommand : Command<DungeonAggregate, DungeonId>
{
    public RemoveDungeonCommand(DungeonId aggregateId, DungeonId dungeonId, TransactionContext context) :
        base(aggregateId)
    {
        DungeonId = dungeonId;
        Context = context;
    }

    public DungeonId DungeonId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class RemoveDungeonCommandHandler : CommandHandler<DungeonAggregate, DungeonId, RemoveDungeonCommand>
{
    public override Task ExecuteAsync(DungeonAggregate aggregate, RemoveDungeonCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.RemoveDungeon(command.DungeonId, command.Context);
        return Task.CompletedTask;
    }
}