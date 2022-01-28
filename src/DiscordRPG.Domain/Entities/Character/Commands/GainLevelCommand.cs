using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class GainLevelCommand : Command<GuildAggregate, GuildId>
{
    public GainLevelCommand(GuildId aggregateId, CharacterId characterId, Level newLevel, Level oldLevel,
        TransactionContext context
    ) :
        base(aggregateId)
    {
        CharacterId = characterId;
        NewLevel = newLevel;
        Context = context;
        OldLevel = oldLevel;
    }

    public CharacterId CharacterId { get; private set; }
    public Level NewLevel { get; private set; }
    public Level OldLevel { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class GainLevelCommandHandler : CommandHandler<GuildAggregate, GuildId, GainLevelCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, GainLevelCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SetCharacterLevel(command.CharacterId, command.NewLevel, command.OldLevel, command.Context);
        return Task.CompletedTask;
    }
}