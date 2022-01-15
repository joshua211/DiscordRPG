using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Character;

public class CharacterId : IIdentity
{
    public CharacterId(string value)
    {
        Value = value;
    }

    public string Value { get; }
}