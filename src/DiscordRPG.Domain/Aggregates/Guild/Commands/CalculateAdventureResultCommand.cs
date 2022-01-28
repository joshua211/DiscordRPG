using DiscordRPG.Common;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Guild.Commands;

public class CalculateAdventureResultCommand : Command<GuildAggregate, GuildId>
{
    public CalculateAdventureResultCommand(GuildId aggregateId, DungeonId dungeonId, CharacterId characterId,
        ActivityDuration duration, TransactionContext context) : base(aggregateId)
    {
        DungeonId = dungeonId;
        CharacterId = characterId;
        Duration = duration;
        Context = context;
    }

    public DungeonId DungeonId { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public ActivityDuration Duration { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class
    CalculateAdventureResultCommandHandler : CommandHandler<GuildAggregate, GuildId, CalculateAdventureResultCommand>
{
    private readonly IAdventureResultService adventureResultService;

    public CalculateAdventureResultCommandHandler(IAdventureResultService adventureResultService)
    {
        this.adventureResultService = adventureResultService;
    }

    public override Task ExecuteAsync(GuildAggregate aggregate, CalculateAdventureResultCommand command,
        CancellationToken cancellationToken)
    {
        adventureResultService.Calculate(aggregate, command.CharacterId, command.DungeonId, command.Duration,
            command.Context);
        return Task.CompletedTask;
    }
}