using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class AddTitleCommand : Command<GuildAggregate, GuildId>
{
    public AddTitleCommand(GuildId aggregateId, Title title, CharacterId characterId, TransactionContext context) :
        base(aggregateId)
    {
        Title = title;
        CharacterId = characterId;
        Context = context;
    }

    public Title Title { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AddTitleCommandHandler : CommandHandler<GuildAggregate, GuildId, AddTitleCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, AddTitleCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddTitle(command.CharacterId, command.Title, command.Context);
        return Task.CompletedTask;
    }
}