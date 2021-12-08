using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class RestoreHealthCommand : Command
{
    public RestoreHealthCommand(Character character, float amountInPercent)
    {
        Character = character;
        AmountInPercent = amountInPercent;
    }

    public Character Character { get; private set; }
    public float AmountInPercent { get; private set; }
}