namespace DiscordRPG.Core.Commands.Characters;

public class UpdateEquipmentCommand : Command
{
    public UpdateEquipmentCommand(Identity charId, EquipmentInfo equipmentInfo)
    {
        CharId = charId;
        EquipmentInfo = equipmentInfo;
    }

    public Identity CharId { get; private set; }
    public EquipmentInfo EquipmentInfo { get; private set; }
}