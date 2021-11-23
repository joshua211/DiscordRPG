using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;

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
    }
}