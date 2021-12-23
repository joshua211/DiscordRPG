namespace DiscordRPG.Core.Commands.Shops;

public class UpdateShopCommand : Command
{
    public UpdateShopCommand(Identity shopId, Identity charId, List<Equipment> equipment)
    {
        ShopId = shopId;
        CharId = charId;
        Equipment = equipment;
    }

    public List<Equipment> Equipment { get; private set; }
    public Identity ShopId { get; private set; }
    public Identity CharId { get; }
}