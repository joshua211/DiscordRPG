using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class GainLevelCommand : Command<CharacterAggregate, CharacterId>
{
    public GainLevelCommand(CharacterId aggregateId, Level newLevel, Level oldLevel,
        TransactionContext context
    ) :
        base(aggregateId)
    {
        NewLevel = newLevel;
        Context = context;
        OldLevel = oldLevel;
    }

    public Level NewLevel { get; private set; }
    public Level OldLevel { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class GainLevelCommandHandler : CommandHandler<CharacterAggregate, CharacterId, GainLevelCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, GainLevelCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.SetCharacterLevel(command.CharacterId, command.NewLevel, command.OldLevel, command.Context);
        return Task.CompletedTask;
    }
}