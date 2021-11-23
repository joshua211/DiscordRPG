using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.Enums;

namespace DiscordRPG.Client.Modules;

[Group("rpg test")]
public class TestModule : ModuleBase<SocketCommandContext>
{
    [Group("activity")]
    public class ActivityModule : ModuleBase<SocketCommandContext>
    {
        private readonly IActivityService activityService;

        public ActivityModule(IActivityService activityService)
        {
            this.activityService = activityService;
        }

        [Command]
        public async Task TestActivityAsync()
        {
            await activityService.QueueActivityAsync(Context.User.Id, DateTime.Now, TimeSpan.FromSeconds(5),
                ActivityType.Unknown);

            await ReplyAsync("Testing activity, check logs");
        }
    }
}