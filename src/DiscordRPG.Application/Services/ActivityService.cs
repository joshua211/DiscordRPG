using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands;
using Hangfire;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class ActivityService : ApplicationService, IActivityService
{
    public ActivityService(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Result> QueueActivityAsync(ulong userId, DateTime start, TimeSpan duration, ActivityType type,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var activity = new Activity(userId, start, duration, ActivityType.Unknown);
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

    public async Task ExecuteActivityAsync(string activityId)
    {
        logger.Information("Executing activity!");
        var activity = await mediator.Send(new GetActivityQuery(activityId));
        if (activity is null)
            return;

        switch (activity.Type)
        {
            case ActivityType.Unknown:
                logger.Information("Executed activity!");
                break;
        }
    }
}