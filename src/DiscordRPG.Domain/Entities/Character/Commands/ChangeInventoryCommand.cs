using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class ChangeInventoryCommand : Command<GuildAggregate, GuildId>
{
    public ChangeInventoryCommand(GuildId id, CharacterId characterId, List<Item> newInventory,
        TransactionContext context) : base(id)
    {
        CharacterId = characterId;
        NewInventory = newInventory;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Item> NewInventory { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ChangeInventoryCommandHandler : CommandHandler<GuildAggregate, GuildId, ChangeInventoryCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, ChangeInventoryCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ChangeCharacterInventory(command.CharacterId, command.NewInventory, command.Context);
        return Task.CompletedTask;
    }
}