using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class KillCharacterCommand : Command<GuildAggregate, GuildId>
{
    public KillCharacterCommand(GuildId aggregateId, CharacterId characterId) : base(aggregateId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}

public class KillCharacterCommandHandler : CommandHandler<GuildAggregate, GuildId, KillCharacterCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, KillCharacterCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.KillCharacter(command.CharacterId);
        return Task.CompletedTask;
    }
}