using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class RestoreHealthCommand : Command
{
    public RestoreHealthCommand(Character character, int amount)
    {
        Character = character;
        Amount = amount;
    }

    public Character Character { get; private set; }
    public int Amount { get; private set; }
}