using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Character;

public class CharacterId : IIdentity
{
    public CharacterId(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is CharacterId id)
            return id.Value == Value;

        return false;
    }
}