using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Activity.Events;
using DiscordRPG.Domain.Aggregates.Activity.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class ActivityReadModel : IMongoDbReadModel, IAmReadModelFor<GuildAggregate, GuildId, ActivityAdded>,
    IAmReadModelFor<GuildAggregate, GuildId, ActivityCancelled>,
    IAmReadModelFor<GuildAggregate, GuildId, ActivityCompleted>
{
    public string CharacterId { get; private set; }
    public string GuildId { get; set; }
    public ActivityDuration Duration { get; private set; }
    public ActivityType Type { get; set; }
    public JobId JobId { get; set; }
    public ActivityData ActivityData { get; private set; }
    public DateTime StartTime { get; private set; }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ActivityAdded> domainEvent)
    {
        var activity = domainEvent.AggregateEvent.Activity;
        Id = activity.Id.Value;
        CharacterId = activity.CharacterId.Value;
        GuildId = domainEvent.AggregateIdentity.Value;
        Duration = activity.Duration;
        Type = activity.Type;
        JobId = activity.JobId;
        ActivityData = activity.ActivityData;
        StartTime = activity.StartTime.Value;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ActivityCancelled> domainEvent)
    {
        context.MarkForDeletion();
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ActivityCompleted> domainEvent)
    {
        context.MarkForDeletion();
    }

    public string Id { get; private set; }
    public long? Version { get; set; }
}