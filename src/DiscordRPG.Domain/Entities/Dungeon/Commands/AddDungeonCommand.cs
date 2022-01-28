using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Dungeon.Commands;

public class AddDungeonCommand : Command<GuildAggregate, GuildId>
{
    public AddDungeonCommand(GuildId aggregateId, Dungeon dungeon, CharacterId characterId, TransactionContext context)
        : base(aggregateId)
    {
        Dungeon = dungeon;
        Context = context;
        CharacterId = characterId;
    }

    public Dungeon Dungeon { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AddDungeonCommandHandler : CommandHandler<GuildAggregate, GuildId, AddDungeonCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddDungeonCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddDungeon(command.Dungeon, command.CharacterId, command.Context);
        return Task.CompletedTask;
    }
}