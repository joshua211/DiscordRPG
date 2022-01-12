using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class CompleteRestCommand : Command<GuildAggregate, GuildId>
{
    public CompleteRestCommand(GuildId aggregateId, CharacterId characterId) : base(aggregateId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}

public class CompleteRestCommandHandler : CommandHandler<GuildAggregate, GuildId, CompleteRestCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CompleteRestCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CompleteCharacterRest(command.CharacterId);
        return Task.CompletedTask;
    }
}