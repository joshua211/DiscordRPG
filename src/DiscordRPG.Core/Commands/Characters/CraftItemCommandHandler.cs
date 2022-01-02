using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class CraftItemCommandHandler : CommandHandler<CraftItemCommand>
{
    private readonly IRepository<Character> repository;

    public CraftItemCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> repository) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(CraftItemCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", command.GetType().Name);
            if (!command.Recipe.IsCraftableWith(command.Character.Inventory))
                return ExecutionResult.Failure($"Not enough ingredients");

            foreach (var (name, amount) in command.Recipe.Ingredients)
            {
                var item = command.Character.Inventory.First(i =>
                    i.Name == name && i.Rarity == command.Recipe.Rarity &&
                    i.Level.RoundOff() == command.Recipe.Level.RoundOff());
                if (item.Amount == amount)
                    command.Character.Inventory.Remove(item);
                else
                    item.Amount -= amount;
            }

            var existing =
                command.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == command.Recipe.Item.GetItemCode());
            if (existing is not null)
                existing.Amount++;
            else
                command.Character.Inventory.Add(command.Recipe.Item);

            await repository.UpdateAsync(command.Character, cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}