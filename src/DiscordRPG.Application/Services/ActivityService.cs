using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Activities;
using Hangfire;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class ActivityService : ApplicationService, IActivityService
{
    public ActivityService(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Result> QueueActivityAsync(string charId, TimeSpan duration, ActivityType type, ActivityData data,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var activity = new Activity(charId, DateTime.Now, duration, type, data);
            var jobId = BackgroundJob.Schedule<ActivityService>(x => ExecuteActivityAsync(activity.ID), duration);
            activity.JobId = jobId;

            var result = await PublishAsync(ctx, new CreateActivityCommand(activity), cancellationToken);
            if (!result.WasSuccessful)
            {
                BackgroundJob.Delete(jobId);
                TransactionWarning(ctx, "Failed to create Activity: {@Activity}", activity);

                return Result.Failure("Failed to create Activity");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<Activity>> GetCharacterActivityAsync(string charId, CancellationToken token)
    {
        using var ctx = TransactionBegin();
        try
        {
            var query = new GetCharacterActivityQuery(charId);
            var activity = await ProcessAsync(ctx, query, token);

            if (activity is null)
            {
                return Result<Activity>.Failure("No activity found");
            }

            return Result<Activity>.Success(activity);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Activity>.Failure(e.Message);
        }
    }

    public async Task ExecuteActivityAsync(string activityId)
    {
        var activity = await mediator.Send(new GetActivityQuery(activityId));
        if (activity is null)
            return;

        switch (activity.Type)
        {
            case ActivityType.Unknown:
                logger.Information("Executed activity!");
                break;
            case ActivityType.SearchDungeon:
                logger.Information("Searched dungeon for player level {Level}", activity.Data.PlayerLevel);
                break;
            default:
                logger.Warning("Activity is not handled, {Name}", activity.Type);
                break;
        }

        await mediator.Send(new DeleteActivityCommand(activityId));
    }
}