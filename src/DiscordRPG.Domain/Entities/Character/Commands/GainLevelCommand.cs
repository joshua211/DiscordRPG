using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class GainLevelCommand : Command<GuildAggregate, GuildId>
{
    public GainLevelCommand(GuildId aggregateId, CharacterId characterId, Level level, TransactionContext context) :
        base(aggregateId)
    {
        CharacterId = characterId;
        Level = level;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public Level Level { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class GainLevelCommandHandler : CommandHandler<GuildAggregate, GuildId, GainLevelCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, GainLevelCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SetCharacterLevel(command.CharacterId, command.Level, command.Context);
        return Task.CompletedTask;
    }
}