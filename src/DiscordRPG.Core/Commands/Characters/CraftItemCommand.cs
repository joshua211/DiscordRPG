using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands.Characters;

public class CraftItemCommand : Command
{
    public CraftItemCommand(Character character, Recipe recipe)
    {
        Character = character;
        Recipe = recipe;
    }

    public Character Character { get; private set; }
    public Recipe Recipe { get; private set; }
}