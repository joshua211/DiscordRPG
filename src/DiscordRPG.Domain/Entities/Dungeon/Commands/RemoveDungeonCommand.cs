using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class RemoveDungeonCommand : Command<GuildAggregate, GuildId>
{
    public RemoveDungeonCommand(GuildId aggregateId, DungeonId dungeonId) : base(aggregateId)
    {
        DungeonId = dungeonId;
    }

    public DungeonId DungeonId { get; private set; }
}

public class RemoveDungeonCommandHandler : CommandHandler<GuildAggregate, GuildId, RemoveDungeonCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, RemoveDungeonCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RemoveDungeon(command.DungeonId);
        return Task.CompletedTask;
    }
}