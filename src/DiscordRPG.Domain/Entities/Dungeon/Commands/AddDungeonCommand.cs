using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class AddDungeonCommand : Command<GuildAggregate, GuildId>
{
    public AddDungeonCommand(GuildId aggregateId, Dungeon dungeon) : base(aggregateId)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
}

public class AddDungeonCommandHandler : CommandHandler<GuildAggregate, GuildId, AddDungeonCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddDungeonCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddDungeon(command.Dungeon);
        return Task.CompletedTask;
    }
}