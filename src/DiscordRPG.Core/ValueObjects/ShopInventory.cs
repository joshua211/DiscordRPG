namespace DiscordRPG.Core.ValueObjects;

public class ShopInventory
{
    public ShopInventory(Identity charId, List<Equipment> equipment)
    {
        CharId = charId;
        Equipment = equipment;
    }

    public Identity CharId { get; private set; }
    public List<Equipment> Equipment { get; private set; }
}