namespace DiscordRPG.Core.ValueObjects;

public class DiscordId
{
    public DiscordId(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static implicit operator ulong(DiscordId id) => ulong.Parse(id.Value);
    public static implicit operator DiscordId(string val) => new DiscordId(val);

    public override string ToString()
    {
        return Value;
    }
}