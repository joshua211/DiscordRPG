using System.Reflection;
using DiscordRPG.Common.Attributes;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Common;

public abstract class DomainEvent : INotification
{
    public DomainEvent()
    {
        DateOccurred = DateTime.Now;
        EventId = Guid.NewGuid();
        EventVersion =
            Attribute.GetCustomAttribute(GetType(), typeof(EventVersionAttribute)) is not EventVersionAttribute attr
                ? 1
                : attr.Version;
        ApplicationVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
    }

    [BsonId] public Guid EventId { get; protected set; }

    public DateTime DateOccurred { get; protected set; }
    public int EventVersion { get; protected set; }
    public string ApplicationVersion { get; protected set; }
}