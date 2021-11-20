namespace DiscordRPG.Common;

public abstract class Entity
{
    public Entity()
    {
        ID = Guid.NewGuid().ToString();
    }

    public string ID { get; set; }
}