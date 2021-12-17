using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Characters;

public class UpdateEquipmentCommandHandler : CommandHandler<UpdateEquipmentCommand>
{
    private readonly IRepository<Character> characterRepository;

    public UpdateEquipmentCommandHandler(IMediator mediator, ILogger logger, IRepository<Character> characterRepository)
        : base(mediator, logger)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(UpdateEquipmentCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var character = await characterRepository.GetAsync(request.CharId, cancellationToken);
            var oldEquip = character.Equipment;
            character.UpdateEquipment(request.EquipmentInfo);

            await characterRepository.UpdateAsync(character, cancellationToken);
            await PublishAsync(new EquipmentChanged(request.CharId, oldEquip, request.EquipmentInfo),
                cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}