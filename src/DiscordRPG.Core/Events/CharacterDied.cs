using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class CharacterDied : DomainEvent
{
    public CharacterDied(Character character, Wound finalWound)
    {
        Character = character;
        FinalWound = finalWound;
    }

    public Character Character { get; private set; }
    public Wound FinalWound { get; private set; }
}