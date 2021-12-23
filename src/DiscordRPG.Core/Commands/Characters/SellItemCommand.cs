using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class SellItemCommand : Command
{
    public SellItemCommand(Character character, Item item)
    {
        Character = character;
        Item = item;
    }

    public Character Character { get; private set; }
    public Item Item { get; private set; }
}