using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class ChangeInventoryCommand : Command<CharacterAggregate, CharacterId>
{
    public ChangeInventoryCommand(CharacterId id, List<Item> newInventory,
        TransactionContext context) : base(id)
    {
        NewInventory = newInventory;
        Context = context;
    }

    public List<Item> NewInventory { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ChangeInventoryCommandHandler : CommandHandler<CharacterAggregate, CharacterId, ChangeInventoryCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, ChangeInventoryCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ChangeCharacterInventory(command.NewInventory, command.Context);
        return Task.CompletedTask;
    }
}