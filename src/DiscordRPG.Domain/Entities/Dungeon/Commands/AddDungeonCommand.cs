using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class AddDungeonCommand : Command<GuildAggregate, GuildId>
{
    public AddDungeonCommand(GuildId aggregateId, Dungeon dungeon, TransactionContext context) : base(aggregateId)
    {
        Dungeon = dungeon;
        Context = context;
    }

    public Dungeon Dungeon { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AddDungeonCommandHandler : CommandHandler<GuildAggregate, GuildId, AddDungeonCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddDungeonCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddDungeon(command.Dungeon, command.Context);
        return Task.CompletedTask;
    }
}