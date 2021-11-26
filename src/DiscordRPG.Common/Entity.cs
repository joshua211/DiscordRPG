using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Common;

public abstract class Entity
{
    public Entity()
    {
        ID = Guid.NewGuid().ToString();
    }

    [BsonId] public Identity ID { get; set; }
}