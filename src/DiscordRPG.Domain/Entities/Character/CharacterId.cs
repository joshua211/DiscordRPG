using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Character;

public class CharacterId : Identity<CharacterId>
{
    public CharacterId(string value) : base(value)
    {
    }
}