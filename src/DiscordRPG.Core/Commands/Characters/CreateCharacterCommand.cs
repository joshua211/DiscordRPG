using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class CreateCharacterCommand : Command
{
    public CreateCharacterCommand(Character character)
    {
        Character = character;
    }

    public Character Character { get; private set; }
}