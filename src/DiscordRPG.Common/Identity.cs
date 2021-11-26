namespace DiscordRPG.Common;

public class Identity
{
    public Identity(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException(nameof(value));

        Value = value;
    }

    public string Value { get; private set; }

    public static implicit operator string(Identity identity) => identity.Value;
    public static implicit operator Identity(string val) => new Identity(val);

    public override string ToString()
    {
        return Value;
    }
}