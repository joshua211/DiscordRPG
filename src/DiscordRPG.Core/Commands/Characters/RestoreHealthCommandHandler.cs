using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class RestoreHealthCommandHandler : CommandHandler<RestoreHealthCommand>
{
    private readonly IRepository<Character> characterRepository;

    public RestoreHealthCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> characterRepository) :
        base(mediator, logger)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(RestoreHealthCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var maxHealth = request.Character.MaxHealth;
            var amountToHeal = (int) (maxHealth * request.AmountInPercent);
            logger.Here().Verbose("Healing {Amount} damage to character {@Character}", amountToHeal, request.Character);

            while (amountToHeal > 0)
            {
                var wound = request.Character.Wounds.FirstOrDefault();
                if (wound is null)
                    break;

                if (amountToHeal > wound.DamageValue)
                {
                    request.Character.Wounds.Remove(wound);
                    amountToHeal -= wound.DamageValue;
                }
                else
                {
                    wound.DamageValue -= amountToHeal;
                    amountToHeal = 0;
                }
            }

            await characterRepository.UpdateAsync(request.Character, cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}