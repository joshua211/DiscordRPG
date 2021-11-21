using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class CharacterCreated : DomainEvent
{
    public CharacterCreated(Character character)
    {
        Character = character;
    }

    public Character Character { get; private set; }
}