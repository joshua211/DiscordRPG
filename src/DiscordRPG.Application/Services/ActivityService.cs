using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Application.Worker;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity;
using DiscordRPG.Domain.Entities.Activity.Commands;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Activity.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using EventFlow;
using EventFlow.Queries;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace DiscordRPG.Application.Services;

public class ActivityService : IActivityService
{
    private readonly ICommandBus bus;
    private readonly IHostEnvironment hostEnvironment;
    private readonly ILogger logger;
    private readonly IQueryProcessor processor;


    public ActivityService(IQueryProcessor processor, ICommandBus bus, ILogger logger, IHostEnvironment hostEnvironment)
    {
        this.processor = processor;
        this.bus = bus;
        this.logger = logger;
        this.hostEnvironment = hostEnvironment;
    }

    public async Task<Result> QueueActivityAsync(GuildId guildId, CharacterId characterId, ActivityDuration duration,
        ActivityType type, ActivityData data,
        TransactionContext context, CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Queuing {Duration} {Type} Activity for Character {ID}", duration, type,
            characterId.Value);
        var timespan = hostEnvironment.IsDevelopment()
            ? TimeSpan.FromSeconds(10)
            : TimeSpan.FromMinutes((int) duration);
        var id = ActivityId.New;
        var jobId = BackgroundJob.Schedule<ActivityWorker>(x => x.ExecuteActivityAsync(id), timespan);

        var activity = new Activity(ActivityId.New, duration, type, new JobId(jobId), data, characterId);
        var cmd = new AddActivityCommand(guildId, activity, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to queue activity with ID {Id} and JobId {JobId}", id.Value, jobId);
            return Result.Failure("Failed to queue activity");
        }

        logger.Context(context).Information("Queued Activity with ID {Id} and JobId {JobId}", id.Value, jobId);

        return Result.Success();
    }

    public async Task<Result<ActivityReadModel>> GetCharacterActivityAsync(CharacterId characterId,
        TransactionContext context, CancellationToken token = default)
    {
        logger.Context(context).Information("Querying Activity for Character with id {Id}", characterId.Value);
        var query = new GetCharacterActivityQuery(characterId);
        var result = await processor.ProcessAsync(query, token);
        logger.Context(context).Information("Found Activity: {@Activity}", result);

        return Result<ActivityReadModel>.Success(result);
    }

    public async Task<Result<ActivityReadModel>> GetActivityAsync(ActivityId id, TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Querying Activity with id {Id}", id.Value);
        var query = new GetActivityQuery(id);
        var result = await processor.ProcessAsync(query, token);
        logger.Context(context).Information("Found Activity: {@Activity}", result);

        return Result<ActivityReadModel>.Success(result);
    }

    public async Task<Result> StopActivityAsync(GuildId guildId, ActivityId activityId, TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Stopping Activity with ID {Id}", activityId.Value);
        var cmd = new CancelActivityCommand(guildId, activityId, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to stop activity");
            return Result.Failure("Failed to stop activity");
        }

        return Result.Success();
    }

    public async Task<Result<IEnumerable<ActivityReadModel>>> GetAllActivitiesAsync(TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Querying all Activities");
        var query = new GetAllActivitiesQuery();
        var result = await processor.ProcessAsync(query, token);

        return Result<IEnumerable<ActivityReadModel>>.Success(result);
    }

    public async Task<Result> CompleteActivityAsync(GuildId guildId, ActivityId activityId, TransactionContext context)
    {
        logger.Context(context).Information("Completing activity with Id {Id}", activityId.Value);
        var cmd = new CompleteActivityCommand(guildId, activityId, true, context);
        var result = await bus.PublishAsync(cmd, CancellationToken.None);

        if (!result.IsSuccess)
        {
            logger.Error("Failed to complete Activity with Id {Id} for Guild {GuildId}", activityId.Value,
                guildId.Value);

            return Result.Failure("Failed to complete activity");
        }

        return Result.Success();
    }
}