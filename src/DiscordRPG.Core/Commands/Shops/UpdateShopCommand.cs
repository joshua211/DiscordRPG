using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Shops;

public class UpdateShopCommand : Command
{
    public UpdateShopCommand(List<Equipment> equipment, Shop shop, Character character)
    {
        Equipment = equipment;
        Shop = shop;
        Character = character;
    }

    public List<Equipment> Equipment { get; private set; }
    public Shop Shop { get; private set; }
    public Character Character { get; private set; }
}