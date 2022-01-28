using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class RemoveDungeonCommand : Command<GuildAggregate, GuildId>
{
    public RemoveDungeonCommand(GuildId aggregateId, DungeonId dungeonId, TransactionContext context) :
        base(aggregateId)
    {
        DungeonId = dungeonId;
        Context = context;
    }

    public DungeonId DungeonId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class RemoveDungeonCommandHandler : CommandHandler<GuildAggregate, GuildId, RemoveDungeonCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, RemoveDungeonCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RemoveDungeon(command.DungeonId, command.Context);
        return Task.CompletedTask;
    }
}