using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.Enums;

namespace DiscordRPG.Client.Modules;

public class SettingsModule : ModuleBase<SocketCommandContext>
{
    private readonly IActivityService activityService;

    public SettingsModule(IActivityService activityService)
    {
        this.activityService = activityService;
    }

    [Command("testActivity")]
    public async Task TestActivity()
    {
        await activityService.QueueActivityAsync(Context.User.Id, DateTime.Now, TimeSpan.FromSeconds(10),
            ActivityType.Unknown);
        await ReplyAsync("Queued activity");
    }
}