using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class RemoveItemCommand : Command
{
    public RemoveItemCommand(string itemCode, int amount, Character character)
    {
        ItemCode = itemCode;
        Amount = amount;
        Character = character;
    }

    public string ItemCode { get; private set; }
    public int Amount { get; private set; }
    public Character Character { get; private set; }
}