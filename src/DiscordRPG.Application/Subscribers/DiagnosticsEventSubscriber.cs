using DiscordRPG.Application.Worker;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class DiagnosticsEventSubscriber : IDomainEventSubscriber<AdventureResultCalculated>,
    IDomainEventSubscriber<DungeonCreated>,
    IDomainEventSubscriber<DungeonDeleted>,
    IDomainEventSubscriber<CharacterDied>,
    IDomainEventSubscriber<ActivityCreated>,
    IDomainEventSubscriber<ActivityDeleted>

{
    private readonly DiagnosticsWorker worker;

    public DiagnosticsEventSubscriber(DiagnosticsWorker worker)
    {
        this.worker = worker;
    }

    public Task Handle(ActivityCreated domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }

    public Task Handle(ActivityDeleted domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }


    public Task Handle(AdventureResultCalculated domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }

    public Task Handle(CharacterDied domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }

    public Task Handle(DungeonCreated domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }

    public Task Handle(DungeonDeleted domainEvent, CancellationToken cancellationToken)
    {
        worker.AddDomainEvent(domainEvent);
        return Task.CompletedTask;
    }
}