using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class BuyItemCommand : Command
{
    public BuyItemCommand(Item item, Character character)
    {
        Item = item;
        Character = character;
    }

    public Item Item { get; private set; }
    public Character Character { get; set; }
}