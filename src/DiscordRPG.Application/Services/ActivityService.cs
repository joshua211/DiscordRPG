using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Application.Worker;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Activities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace DiscordRPG.Application.Services;

public class ActivityService : ApplicationService, IActivityService
{
    private readonly IChannelManager channelManager;
    private readonly IGuildService guildService;
    private readonly IHostEnvironment hostEnvironment;

    public ActivityService(IMediator mediator, ILogger logger, IGuildService guildService,
        IChannelManager channelManager, IHostEnvironment hostEnvironment) : base(mediator, logger)
    {
        this.guildService = guildService;
        this.channelManager = channelManager;
        this.hostEnvironment = hostEnvironment;
    }

    public async Task<Result> QueueActivityAsync(Identity charId, ActivityDuration duration, ActivityType type,
        ActivityData data,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            TimeSpan timespan;
            if (hostEnvironment.IsDevelopment())
                timespan = TimeSpan.FromSeconds(10);
            else
                timespan = TimeSpan.FromMinutes((int) duration);

            var activity = new Activity(charId, DateTime.Now, duration, type, data);
            var jobId = BackgroundJob.Schedule<ActivityWorker>(x => x.ExecuteActivityAsync(activity.ID), timespan);
            activity.JobId = jobId;

            var result = await PublishAsync(ctx, new CreateActivityCommand(activity), cancellationToken);
            if (!result.WasSuccessful)
            {
                BackgroundJob.Delete(jobId);
                TransactionError(ctx, "Failed to create Activity: {@Activity}", activity);

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

    public async Task<Result<Activity>> GetCharacterActivityAsync(Identity charId,
        TransactionContext parentContext = null, CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
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

    public async Task<Result<Activity>> GetActivityAsync(Identity activityId, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetActivityQuery(activityId);
            var result = await ProcessAsync(ctx, query, token);

            if (result is null)
            {
                TransactionWarning(ctx, "No Activity found with id {Id}", activityId);
                return Result<Activity>.Failure("No activity found");
            }

            return Result<Activity>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Activity>.Failure(e.Message);
        }
    }

    public async Task<Result> StopActivityAsync(Activity dialogActivity, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var cmd = new DeleteActivityCommand(dialogActivity.ID);
            var result = await PublishAsync(ctx, cmd, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result.Failure("Something went wrong while trying to stop the activity");
            }

            if (BackgroundJob.Delete(dialogActivity.JobId))
                return Result.Success();

            TransactionError(ctx, "Failed to delete background job with ID {Id} for activity {ActId}",
                dialogActivity.JobId, dialogActivity.ID);

            return Result.Failure("Something went wrong while trying to stop the activity");
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }
}