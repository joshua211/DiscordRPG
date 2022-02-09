using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class AddTitleCommand : Command<CharacterAggregate, CharacterId>
{
    public AddTitleCommand(CharacterId aggregateId, Title title, TransactionContext context) :
        base(aggregateId)
    {
        Title = title;
        Context = context;
    }

    public Title Title { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class AddTitleCommandHandler : CommandHandler<CharacterAggregate, CharacterId, AddTitleCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, AddTitleCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddTitle(command.Title, command.Context);
        return Task.CompletedTask;
    }
}