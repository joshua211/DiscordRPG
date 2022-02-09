using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character;
using DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Commands;

public class CreateDungeonCommand : Command<DungeonAggregate, DungeonId>
{
    public CreateDungeonCommand(DungeonId aggregateId, DungeonName name, Explorations explorations, DungeonLevel level,
        Aspect aspect, Rarity rarity, CharacterId owner, TransactionContext context) : base(aggregateId)
    {
        Name = name;
        Explorations = explorations;
        Level = level;
        Aspect = aspect;
        Rarity = rarity;
        Owner = owner;
        Context = context;
    }

    public DungeonName Name { get; private set; }
    public Explorations Explorations { get; private set; }
    public DungeonLevel Level { get; private set; }
    public Rarity Rarity { get; private set; }
    public Aspect Aspect { get; private set; }
    public CharacterId Owner { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CreateDungeonCommandHandler : CommandHandler<DungeonAggregate, DungeonId, CreateDungeonCommand>
{
    public override Task ExecuteAsync(DungeonAggregate aggregate, CreateDungeonCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.AddDungeon(command.Dungeon, command.DungeonId, command.Context);
        return Task.CompletedTask;
    }
}