using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Common;

public abstract class Entity
{
    public Entity()
    {
        ID = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    [BsonId] public Identity ID { get; set; }

    /// <summary>
    /// Date and time this entity was created, in UTC
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time this entity was last modified, in UTC
    /// </summary>
    public DateTime LastModified { get; set; }
}