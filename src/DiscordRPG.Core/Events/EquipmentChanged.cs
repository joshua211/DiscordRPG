namespace DiscordRPG.Core.Events;

public class EquipmentChanged : DomainEvent
{
    public EquipmentChanged(Identity identity, EquipmentInfo oldEquipment, EquipmentInfo newEquipment)
    {
        Identity = identity;
        OldEquipment = oldEquipment;
        NewEquipment = newEquipment;
    }

    public Identity Identity { get; private set; }
    public EquipmentInfo OldEquipment { get; private set; }
    public EquipmentInfo NewEquipment { get; private set; }
}