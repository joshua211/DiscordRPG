namespace DiscordRPG.Common;

public abstract class DomainEvent
{
    public DomainEvent()
    {
        DateOccurred = DateTime.Now;
        EventId = Guid.NewGuid();
    }

    public DateTime DateOccurred { get; protected set; }
    public Guid EventId { get; protected set; }
}